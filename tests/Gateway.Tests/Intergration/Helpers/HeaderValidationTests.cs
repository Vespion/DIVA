using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using VespionSoftworks.DIVA.Gateway.Tests.Helpers;
using Xunit.Abstractions;

namespace VespionSoftworks.DIVA.Gateway.Tests.Intergration.Helpers;

public abstract class HeaderValidationTests(string targetUrl, ITestOutputHelper testOutputHelper)
{
	private HttpRequestMessage CreateRequest(string timestamp, string sig, [CallerMemberName]string userAgent = "VespionSoftworks.DIVA.Gateway.Tests")
	{
		var content = new StringContent(HeaderValidationValues.ExampleJson);
		content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

		var request = new HttpRequestMessage(HttpMethod.Post, targetUrl)
		{
			Content = content
		};

		request.Headers.Add("X-Signature-Timestamp", timestamp);
		request.Headers.Add("X-Signature-Ed25519", sig);
		request.Headers.Add("User-Agent", $"xUnit Test ({userAgent})");

		return request;
	}

	private TestWebApplicationFactory<Program> CreateServer(string publicKey, bool validate = true)
	{
		var factory = new TestWebApplicationFactory<Program>();
		factory.TestOutput = testOutputHelper;
		factory.PreserveHeaderValidation = true;
		factory.AppConfigurationHook = (_, builder) =>
		{
			builder.AddInMemoryCollection(new Dictionary<string, string?>
			{
				{"Authentication:Discord:Bot:PublicKey", publicKey},
				{"Authentication:Discord:Bot:ValidateHeaders", validate.ToString()}
			});
		};

		return factory;
	}

	[Fact]
	public async Task DoesNotReturnUnauthorizedWhenHeadersValid()
	{
		await using var testServer = CreateServer(HeaderValidationValues.ValidPublicKey);

		var httpClient = testServer.CreateClient();
		var response = await httpClient.SendAsync(CreateRequest(HeaderValidationValues.ValidTimestamp, HeaderValidationValues.ValidSig));

		response.StatusCode
			.Should()
			.NotBe(HttpStatusCode.Unauthorized)
			.And
			.NotBe(HttpStatusCode.Forbidden)
			.And
			.NotBe(HttpStatusCode.InternalServerError);
	}

	[Theory, PairwiseData]
	public async Task ReturnsUnauthorizedWhenHeadersInvalid(
		[CombinatorialValues(
			HeaderValidationValues.ValidTimestamp,
			HeaderValidationValues.InvalidFutureTimestamp,
			HeaderValidationValues.InvalidPastTimestamp,
			"",
			" ",
			"	"
		)] string timestamp,
		[CombinatorialValues(
			HeaderValidationValues.ValidSig,
			HeaderValidationValues.InvalidSig,
			"",
			" ",
			"	"
		)]
		string sig,
		[CombinatorialValues(
			HeaderValidationValues.ValidPublicKey,
			HeaderValidationValues.InvalidPublicKey,
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

		await using var testServer = CreateServer(publicKey);
		var httpClient = testServer.CreateClient();

		var response = await httpClient.SendAsync(CreateRequest(timestamp, sig));

		var responseContent = await response.Content.ReadAsStringAsync();

		responseContent.Should().BeEmpty();

		response.StatusCode
			.Should()
			.Be(HttpStatusCode.Unauthorized);
	}

	[Theory, PairwiseData]
	public async Task IgnoresInvalidHeadersIfConfigured(
		[CombinatorialValues(
			HeaderValidationValues.ValidTimestamp,
			HeaderValidationValues.InvalidFutureTimestamp,
			HeaderValidationValues.InvalidPastTimestamp,
			"",
			" ",
			"	"
		)] string timestamp,
		[CombinatorialValues(
			HeaderValidationValues.ValidSig,
			HeaderValidationValues.InvalidSig,
			"",
			" ",
			"	"
		)]
		string sig,
		[CombinatorialValues(
			HeaderValidationValues.ValidPublicKey,
			HeaderValidationValues.InvalidPublicKey,
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

		await using var testServer = CreateServer(publicKey, false);
		var httpClient = testServer.CreateClient();

		var response = await httpClient.SendAsync(CreateRequest(timestamp, sig));

		response.StatusCode
			.Should()
			.NotBe(HttpStatusCode.Unauthorized)
			.And
			.NotBe(HttpStatusCode.Forbidden)
			.And
			.NotBe(HttpStatusCode.InternalServerError);
	}
}