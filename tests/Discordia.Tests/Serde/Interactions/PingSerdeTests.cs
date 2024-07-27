using System.Text.Json;
using FluentAssertions;
using VespionSoftworks.DIVA.Discordia.API.Models;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Base;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Ping;
using VespionSoftworks.DIVA.Discordia.Models;

namespace VespionSoftworks.DIVA.Discordia.Tests.Serde.Interactions;

public class PingSerdeTests
{
	private const string ExamplePingRequestJson = """
	{
	  "app_permissions": "562949953601536",
	  "application_id": "1266083078506418176",
	  "authorizing_integration_owners": {},
	  "entitlements": [],
	  "id": "1266118899867586562",
	  "token": "aW50ZXJhY3Rpb246MTI2NjExODg5OTg2NzU4NjU2Mjo0OEREdjNBc0R5d3pPMkh0Y2xsb0JtR1QySGc0T1hjU29RbHNvMDR5UFhuZzNVN2ZrOFowN1Bja0dwZEVwS0dZUm03YTZhUzNFeUc4aW5TTUtEQ0VBOXl6WWZhZFlZREZCN2UyUVZwWWxYYlNlRmVkTkxUVEdyMG5NSUZrQ0hXUw",
	  "type": 1,
	  "user": {
		"avatar": "c6a249645d46209f337279cd2ca998c7",
		"avatar_decoration_data": null,
		"bot": true,
		"clan": null,
		"discriminator": "0000",
		"global_name": "Discord",
		"id": "643945264868098049",
		"public_flags": 1,
		"system": true,
		"username": "discord"
	  },
	  "version": 1
	}
	""";

	private static readonly PingRequest ExpectedPingRequestObject = new(
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

	private static readonly LightweightInteractionMessage ExpectedLightweightObject = new(
		new SnowflakeId(1266118899867586562),
		InteractionTypes.Ping
	);

	[Fact]
	public void DeserializesToExpectedObject()
	{
		var parsedObject = JsonSerializer.Deserialize(ExamplePingRequestJson, ModelJsonContext.Default.PingRequest);

		parsedObject
			.Should()
			.BeEquivalentTo(ExpectedPingRequestObject);
	}

	[Fact]
	public void DeserializesToLightweightObject()
	{
		var parsedObject = JsonSerializer.Deserialize(ExamplePingRequestJson, GatewayModelJsonContext.Default.LightweightInteractionMessage);

		parsedObject
			.Should()
			.BeEquivalentTo(ExpectedLightweightObject);
	}

	// [Fact]
	// public void PolymorphiclyDeserializesToExpectedObject()
	// {
	// 	var parsedObject = JsonSerializer.Deserialize<IInteractionsMessage>(ExamplePingRequestJson, ModelJsonContext.Default.Options);
	//
	// 	parsedObject.Should()
	// 		.BeOfType<PingRequest>()
	// 		.Which
	// 		.Should()
	// 		.BeEquivalentTo(ExpectedPingRequestObject);
	// }
}