using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Base;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Ping;
using VespionSoftworks.DIVA.Spine.Endpoints;

namespace VespionSoftworks.DIVA.Spine.Tests.Unit.Endpoints;

public class InteractionsTests
{
	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(99)]
	public async Task Returns404ForUnknownInteractionType(int type)
	{
		// Arrange
		var message = new LightweightInteractionMessage
		{
			Type = (InteractionTypes)type
		};

		// Act
		var result = await InteractionsEndpoint.Root(message);

		// Assert
		result
			.Result
			.Should()
			.BeOfType<NotFound>();
	}

	[Fact]
	public async Task ReturnsFormattedPongResponse()
	{
		// Arrange
		var message = new LightweightInteractionMessage
		{
			Type = InteractionTypes.Ping
		};

		// Act
		var result = await InteractionsEndpoint.Root(message);

		// Assert
		result
			.Result
			.Should()
			.BeOfType<Ok<IInteractionResponse>>()
			.Which
			.Value
			.Should()
			.BeOfType<PingResponse>();
	}
}