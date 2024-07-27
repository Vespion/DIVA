using System.Globalization;
using Discordia.API.Models;
using VespionSoftworks.DIVA.Discordia.API.Models.Entitlements;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Base;
using VespionSoftworks.DIVA.Discordia.Models;

namespace VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Ping;

public readonly record struct PingRequest(
	SnowflakeId Id,
	SnowflakeId ApplicationId,
	Guild? Guild,
	SnowflakeId? GuildId,
	string Token,
	uint Version,
	PermissionFlags? AppPermissions,
	CultureInfo? Locale,
	CultureInfo? GuildLocale,
	AppEntitlement[]? Entitlements,
	Dictionary<AppIntegrationType, SnowflakeId>? AuthorizingIntegrationOwners,
	InteractionContexts? Context
) : IInteractionsMessage;
