using System.Globalization;
using System.Text.Json.Serialization;
using VespionSoftworks.DIVA.Discordia.API.Models.Entitlements;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Ping;
using VespionSoftworks.DIVA.Discordia.Models;

namespace VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Base;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(typeof(PingRequest), (int)InteractionTypes.Ping)]
public interface IInteractionsMessage: IHaveSnowflakeId
{
	[JsonPropertyName("application_id")]
	public SnowflakeId ApplicationId { get; }

	[JsonPropertyName("guild")]
	public Guild? Guild { get; }

	[JsonPropertyName("guild_id")]
	public SnowflakeId? GuildId { get; }

	[JsonPropertyName("token")]
	public string Token { get; }

	[JsonPropertyName("version")]
	public uint Version { get; }

	[JsonPropertyName("app_permissions")]
	public PermissionFlags? AppPermissions { get; }

	[JsonPropertyName("locale")]
	public CultureInfo? Locale { get; }

	[JsonPropertyName("guild_locale")]
	public CultureInfo? GuildLocale { get; }

	[JsonPropertyName("entitlements")]
	public AppEntitlement[]? Entitlements { get; }

	[JsonPropertyName("authorizing_integration_owners")]
	public Dictionary<AppIntegrationType, SnowflakeId>? AuthorizingIntegrationOwners { get; }

	[JsonPropertyName("context")]
	public InteractionContexts? Context { get; }
}

public interface IInteractionsMessage<out T> : IInteractionsMessage
{
	[JsonPropertyName("data")]
	public T? Data { get; }
}