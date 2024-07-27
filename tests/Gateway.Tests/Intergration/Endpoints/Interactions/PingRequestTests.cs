using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using VespionSoftworks.DIVA.Discordia.API.Models;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Ping;
using VespionSoftworks.DIVA.Discordia.Models;
using VespionSoftworks.DIVA.Gateway.Endpoints;
using VespionSoftworks.DIVA.Gateway.Tests.Intergration.Helpers;
using Xunit.Abstractions;

namespace VespionSoftworks.DIVA.Gateway.Tests.Intergration.Endpoints.Interactions;

public class PingRequestTests : IClassFixture<TestWebApplicationFactory<Program>>
{
	private readonly TestWebApplicationFactory<Program> _factory;
	private readonly HttpClient _client;

	public PingRequestTests(TestWebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
	{
		_factory = factory;
		_factory.TestOutput = testOutputHelper;
		_client = _factory.CreateClient();
	}

	private static readonly PingRequest PingRequest = new(
		new SnowflakeId(1266118899867586562),
		new SnowflakeId(1266083078506418176),
		null,
		null,
		"aW50ZXJhY3Rpb246MTI2NjExODg5OTg2NzU4NjU2Mjo0OEREdjNBc0R5d3pPMkh0Y2xsb0JtR1QySGc0T1hjU29RbHNvMDR5UFhuZzNVN2ZrOFowN1Bja0dwZEVwS0dZUm03YTZhUzNFeUc4aW5TTUtEQ0VBOXl6WWZhZFlZREZCN2UyUVZwWWxYYlNlRmVkTkxUVEdyMG5NSUZrQ0hXUw",
		1,
		PermissionFlags.EmbedLinks | PermissionFlags.MentionEveryone | PermissionFlags.AttachFiles | PermissionFlags.SendPolls,
		null,
		null,
		[],
		new Dictionary<AppIntegrationType, SnowflakeId>(0),
		null
	);

	[Fact]
	public async Task ReturnsPong()
	{
		var response = await _client.PostAsJsonAsync(
			InteractionsEndpoint.routePrefix,
			PingRequest,
			ModelJsonContext.Default.IInteractionsMessage
		);

		response.StatusCode.Should().Be(HttpStatusCode.OK);

		var responseContent = await response.Content.ReadFromJsonAsync<PingResponse>();

		responseContent.Should().NotBeNull();
	}
}