using Microsoft.AspNetCore.Http.HttpResults;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Base;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Ping;
using VespionSoftworks.DIVA.Spine.Auth;
using VespionSoftworks.DIVA.Spine.Utilities.Results;

namespace VespionSoftworks.DIVA.Spine.Endpoints;

internal static class InteractionsEndpoint
{
	internal const string routePrefix = "/interactions";

	public static void RegisterInteractionEndpoints(this WebApplication app)
	{
		var group = app.MapGroup(routePrefix);

		group.RequireAuthorization(builder => builder
			.AddDiscordAuthenticationScheme()
			.RequireAuthenticatedUser()
		);

		group.MapPost("/", Root);

	}

	internal static async Task<Results<
		Ok<IInteractionResponse>,
		NotFound,
		UnauthorizedHttpResult,
		GatewayTimeout,
		BadGateway>
	> Root(LightweightInteractionMessage message)
	{
		switch (message.Type)
		{
			case InteractionTypes.Ping:
				return TypedResults.Ok<IInteractionResponse>(new PingResponse());
			default:
				return TypedResults.NotFound();
		}
	}
}