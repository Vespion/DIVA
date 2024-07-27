using System.Text;
using FluentResults;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Primitives;
using NSec.Cryptography;

namespace VespionSoftworks.DIVA.Gateway.Auth;

internal interface IDiscordHeaderValidator
{
	Task<Result> ValidateHeaders(HttpRequest req, string? rawPublicKey);
}

internal class DiscordHeaderValidator(
	ILogger<DiscordHeaderValidator> logger,
	TelemetryClient telemetry
) : IDiscordHeaderValidator
{
	private enum ValidationErrors
	{
		MultipleSignatures,
		MissingSignature,
		InvalidSignature,
		InvalidTimestamp,
		MissingTimestamp,
		MultipleTimestamps,
		InvalidPublicKey,

		MissingBody,
		VerificationFailed
	}

	private static Error ValidationErrorToError(ValidationErrors error) =>
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
		error switch
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
		{
			ValidationErrors.MultipleSignatures => new Error("Request has multiple signatures"),
			ValidationErrors.MissingSignature => new Error("Request has no signature header"),
			ValidationErrors.InvalidSignature => new Error("Request has an invalid signature"),
			ValidationErrors.InvalidTimestamp => new Error("Request has an invalid timestamp"),
			ValidationErrors.MissingTimestamp => new Error("Request has no timestamp header"),
			ValidationErrors.MultipleTimestamps => new Error("Request has multiple timestamps"),
			ValidationErrors.InvalidPublicKey => new Error("Failed to load public key from options"),
			ValidationErrors.MissingBody => new Error("Request body is empty or missing"),
			ValidationErrors.VerificationFailed => new Error("Failed to validate request signature")
		};

	private readonly record struct SignatureData(byte[]? Signature, ulong? Timestamp, PublicKey? PublicKey);

	private Result<byte[]> ParseSignature(StringValues header)
	{
		if (header.Count > 1)
		{
			logger.LogError("Request has multiple signatures, rejecting");
			return Result.Fail(ValidationErrorToError(ValidationErrors.MultipleSignatures));
		}

		if (header.Count < 1)
		{
			logger.LogError("Request has no signature header, rejecting");
			return Result.Fail(ValidationErrorToError(ValidationErrors.MissingSignature));
		}

		var sig = header[0];
		if (string.IsNullOrWhiteSpace(sig))
		{
			logger.LogError("Request signature header value is invalid, rejecting");
			return Result.Fail(ValidationErrorToError(ValidationErrors.InvalidSignature));
		}

		try
		{
			return Result.Ok(Convert.FromHexString(sig));
		}
		catch (FormatException e)
		{
			logger.LogError(e, "Failed to parse signature hex from header value, rejecting");
			return Result.Fail(ValidationErrorToError(ValidationErrors.InvalidSignature));
		}
	}

	private Result<ulong> ParseTimestamp(StringValues header)
	{
		if (header.Count > 1)
		{
			logger.LogError("Request has multiple timestamps, rejecting");
			return Result.Fail(ValidationErrorToError(ValidationErrors.MultipleTimestamps));
		}

		if (header.Count < 1)
		{
			logger.LogError("Request has no timestamp header, rejecting");
			return Result.Fail(ValidationErrorToError(ValidationErrors.MissingTimestamp));
		}

		var stamp = header[0];
		if (string.IsNullOrWhiteSpace(stamp))
		{
			logger.LogError("Request timestamp header is invalid, rejecting");
			return Result.Fail(ValidationErrorToError(ValidationErrors.InvalidTimestamp));
		}

		if (!ulong.TryParse(stamp, out var timestamp))
		{
			logger.LogError("Request has an invalid timestamp, rejecting");
			return Result.Fail(ValidationErrorToError(ValidationErrors.InvalidTimestamp));
		}

		return Result.Ok(timestamp);
	}

	private Result<PublicKey> ParsePublicKey(string? publicKey)
	{
		if (string.IsNullOrWhiteSpace(publicKey))
		{
			logger.LogError("Failed to load public key from options, rejecting");
			return Result.Fail(ValidationErrorToError(ValidationErrors.InvalidPublicKey));
		}

		byte[] publicKeyHex;
		try
		{
			publicKeyHex = Convert.FromHexString(publicKey);
		}
		catch (FormatException e)
		{
			logger.LogError(e, "Failed to parse public key hex from header value, rejecting");
			return Result.Fail(ValidationErrorToError(ValidationErrors.InvalidPublicKey));
		}

		if (
			!PublicKey.TryImport(
				SignatureAlgorithm.Ed25519,
				publicKeyHex,
				KeyBlobFormat.RawPublicKey,
				out var parsedPublicKey
			)
		)
		{
			logger.LogError("Failed to load public key from options, rejecting");
			return Result.Fail(ValidationErrorToError(ValidationErrors.InvalidPublicKey));
		}

		return Result.Ok(parsedPublicKey!);
	}

	private Result<SignatureData> ParseHeaders(HttpRequest req, string? rawPublicKey)
	{
		var signature = ParseSignature(req.Headers["X-Signature-Ed25519"]);
		var timestamp = ParseTimestamp(req.Headers["X-Signature-Timestamp"]);
		var publicKey = ParsePublicKey(rawPublicKey);

		var results = Result.Merge(signature, timestamp, publicKey);

		if (results.IsFailed)
		{
			return results;
		}

		return new SignatureData(signature.Value, timestamp.Value, publicKey.Value);
	}

	public async Task<Result> ValidateHeaders(HttpRequest req, string? rawPublicKey)
	{
		logger.LogDebug("Validating security headers");

		var (_, parseFailed, (signature, timestamp, publicKey), parseErrors) = ParseHeaders(req, rawPublicKey);

		if (parseFailed)
		{
			return Result.Fail(parseErrors);
		}

		if (!req.ContentLength.HasValue)
		{
			logger.LogError("Request has no content length, thus no body to validate");
			return Result.Fail(ValidationErrorToError(ValidationErrors.MissingBody));
		}

		logger.LogDebug("Reading request body into memory stream");

		logger.LogTrace("Enabling request buffering");
		req.EnableBuffering();

		using var bodyStream = new MemoryStream();
		await req.Body.CopyToAsync(bodyStream);
		req.Body.Seek(0, SeekOrigin.Begin);

		logger.LogTrace("Building buffer for signature validation");
		var writer = new StringWriter();
		await writer.WriteAsync(timestamp!.Value.ToString());
		await writer.WriteAsync(Encoding.UTF8.GetString(bodyStream.ToArray()));
		await writer.FlushAsync();

		var ciphertext = writer.ToString();
		var cipherBuffer = Encoding.UTF8.GetBytes(ciphertext);

		logger.LogInformation("Validating request signature ({Signature}) with key ({PublicKey})",
			Convert.ToHexString(signature!), Convert.ToHexString(publicKey!.Export(KeyBlobFormat.RawPublicKey)));

		bool valid;
		try
		{
			valid = SignatureAlgorithm.Ed25519.Verify(publicKey, cipherBuffer, signature);
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed to validate request signature");
			telemetry.TrackException(e);
			return Result.Fail(ValidationErrorToError(ValidationErrors.VerificationFailed).CausedBy(e));
		}

		return Result.OkIf(
			valid,
			ValidationErrorToError(ValidationErrors.VerificationFailed)
		);
	}
}