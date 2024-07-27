using System.Net;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using VespionSoftworks.DIVA.Gateway.Auth;
using VespionSoftworks.DIVA.Gateway.Tests.Helpers;
using Xunit.Abstractions;
using Xunit.Logging;
using LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory;

namespace VespionSoftworks.DIVA.Gateway.Tests.Unit.Auth;

public class InteractionsHeaderValidation(ITestOutputHelper outputHelper)
{
	[Fact]
	public async Task PassesValidHeaders()
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", HeaderValidationValues.ValidTimestamp},
				{"X-Signature-Ed25519", HeaderValidationValues.ValidSig}
			}),
			HeaderValidationValues.ValidPublicKey
		);

		result.Should().BeSuccess();
	}

	[Fact]
	public async Task FailsWithEmptyBody()
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var httpContext = new DefaultHttpContext();
		httpContext.Request.Headers["X-Signature-Timestamp"] = HeaderValidationValues.ValidTimestamp;
		httpContext.Request.Headers["X-Signature-Ed25519"] = HeaderValidationValues.ValidSig;

		var result = await validator.ValidateHeaders(
			httpContext.Request,
			HeaderValidationValues.ValidPublicKey
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Request body is empty or missing");
	}

	[Fact]
	public async Task FailsWhenTimestampMissing()
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Ed25519", HeaderValidationValues.ValidSig}
			}),
			HeaderValidationValues.ValidPublicKey
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Request has no timestamp header");
	}

	[Theory, CombinatorialData]
	public async Task FailsWhenMultipleTimestamps(
		[CombinatorialValues(
			HeaderValidationValues.ValidTimestamp,
			HeaderValidationValues.InvalidFutureTimestamp,
			HeaderValidationValues.InvalidPastTimestamp
		)]
		string timestamp1,
		[CombinatorialValues(
			HeaderValidationValues.ValidTimestamp,
			HeaderValidationValues.InvalidFutureTimestamp,
			HeaderValidationValues.InvalidPastTimestamp
		)]
		string timestamp2
	)
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", new StringValues([timestamp1, timestamp2])},
				{"X-Signature-Ed25519", HeaderValidationValues.ValidSig}
			}),
			HeaderValidationValues.ValidPublicKey
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Request has multiple timestamps");
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("   ")]
	[InlineData("\t")]
	public async Task FailsWhenTimestampEmpty(string? timestamp)
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", timestamp},
				{"X-Signature-Ed25519", HeaderValidationValues.ValidSig}
			}),
			HeaderValidationValues.ValidPublicKey
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Request has an invalid timestamp");
	}

	[Theory]
	[InlineData("-1")]
	[InlineData("-7851")]
	[InlineData("1,000,000")]
	[InlineData("-1,000,000")]
	[InlineData("hello_world")]
	public async Task FailsWhenTimestampFormatInvalid(string timestamp)
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", timestamp},
				{"X-Signature-Ed25519", HeaderValidationValues.ValidSig}
			}),
			HeaderValidationValues.ValidPublicKey
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Request has an invalid timestamp");
	}

	[Theory]
	[InlineData(HeaderValidationValues.InvalidFutureTimestamp)]
	[InlineData(HeaderValidationValues.InvalidPastTimestamp)]
	public async Task FailsWhenTimestampInvalid(string timestamp)
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", timestamp},
				{"X-Signature-Ed25519", HeaderValidationValues.ValidSig}
			}),
			HeaderValidationValues.ValidPublicKey
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Failed to validate request signature");
	}

	[Fact]
	public async Task FailsWhenSignatureMissing()
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", HeaderValidationValues.ValidTimestamp}
			}),
			HeaderValidationValues.ValidPublicKey
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Request has no signature header");
	}

	[Theory, CombinatorialData]
	public async Task FailsWhenMultipleSignatures(
		[CombinatorialValues(
			HeaderValidationValues.ValidSig,
			HeaderValidationValues.InvalidSig
		)]
		string sig1,
		[CombinatorialValues(
			HeaderValidationValues.ValidSig,
			HeaderValidationValues.InvalidSig
		)]
		string sig2
	)
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", HeaderValidationValues.ValidTimestamp},
				{"X-Signature-Ed25519", new StringValues([sig1, sig2])}
			}),
			HeaderValidationValues.ValidPublicKey
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Request has multiple signatures");
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("   ")]
	[InlineData("\t")]
	public async Task FailsWhenSignatureEmpty(string? sig)
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", HeaderValidationValues.ValidTimestamp},
				{"X-Signature-Ed25519", sig}
			}),
			HeaderValidationValues.ValidPublicKey
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Request has an invalid signature");
	}

	[Theory]
	[InlineData("hello_world")]
	[InlineData($"-{HeaderValidationValues.ValidTimestamp}")]
	public async Task FailsWhenSignatureFormatInvalid(string sig)
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", HeaderValidationValues.ValidTimestamp},
				{"X-Signature-Ed25519", sig}
			}),
			HeaderValidationValues.ValidPublicKey
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Request has an invalid signature");
	}

	[Theory]
	[InlineData(HeaderValidationValues.InvalidSig)]
	public async Task FailsWhenSignatureInvalid(string sig)
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", HeaderValidationValues.ValidTimestamp},
				{"X-Signature-Ed25519", sig}
			}),
			HeaderValidationValues.ValidPublicKey
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Failed to validate request signature");
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData("   ")]
	[InlineData("\t")]
	public async Task FailsWhenPublicKeyEmpty(string? key)
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", HeaderValidationValues.ValidTimestamp},
				{"X-Signature-Ed25519", HeaderValidationValues.ValidSig}
			}),
			key
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Failed to load public key from options");
	}

	[Theory]
	[InlineData("9aba5c14322e9266dfcfd34908976288")]
	[InlineData("3809b6c7b8a0ab55696536b4a4a6ada3")]
	[InlineData("hello_world")]
	[InlineData($"-{HeaderValidationValues.ValidTimestamp}")]
	public async Task FailsWhenPublicKeyFormatInvalid(string key)
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", HeaderValidationValues.ValidTimestamp},
				{"X-Signature-Ed25519", HeaderValidationValues.ValidSig}
			}),
			key
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Failed to load public key from options");
	}

	[Theory]
	[InlineData(HeaderValidationValues.InvalidPublicKey)]
	public async Task FailsWhenPublicKeyInvalid(string key)
	{
		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", HeaderValidationValues.ValidTimestamp},
				{"X-Signature-Ed25519", HeaderValidationValues.ValidSig}
			}),
			key
		);

		result
			.Should()
			.BeFailure()
			.Which
			.Should()
			.HaveError("Failed to validate request signature");
	}

	[Theory, CombinatorialData]
	public async Task FailsWithMultipleInvalidProperties(
		[CombinatorialValues(
			HeaderValidationValues.InvalidFutureTimestamp,
			HeaderValidationValues.InvalidPastTimestamp,
			null,
			"",
			" ",
			"	"
		)] string timestamp,
		[CombinatorialValues(
			HeaderValidationValues.InvalidSig,
			null,
			"",
			" ",
			"	"
		)]
		string sig,
		[CombinatorialValues(
			HeaderValidationValues.InvalidPublicKey,
			null,
			"",
			" ",
			"	"
		)]
		string publicKey
	)
	{
		// Validate
		if(sig == HeaderValidationValues.ValidSig && timestamp == HeaderValidationValues.ValidTimestamp && publicKey == HeaderValidationValues.ValidPublicKey)
		{
			// This combination is valid, so return otherwise the test will fail
			return;
		}

		var loggerFactory = LoggerFactory.Create(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(outputHelper));
		});

		var validator = new DiscordHeaderValidator(
			loggerFactory.CreateLogger<DiscordHeaderValidator>(),
			Objects.InitializeMockTelemetryChannel()
		);

		var result = await validator.ValidateHeaders(
			HeaderValidationValues.ExampleJson.AsHttpRequest(new HeaderDictionary
			{
				{"X-Signature-Timestamp", timestamp},
				{"X-Signature-Ed25519", sig}
			}),
			publicKey
		);

		result.Should().BeFailure();
	}
}