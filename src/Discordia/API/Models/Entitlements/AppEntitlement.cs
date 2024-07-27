using Discordia.Models;
using VespionSoftworks.DIVA.Discordia.Models;

namespace VespionSoftworks.DIVA.Discordia.API.Models.Entitlements;

public readonly record struct AppEntitlement(
	SnowflakeId Id,
	SnowflakeId SkuId,
	SnowflakeId ApplicationId,
	SnowflakeId? UserId,
	EntitlementTypes Type,
	bool Deleted,
	DateTimeOffset? StartsAt,
	DateTimeOffset? EndsAt,
	SnowflakeId? GuildId,
	bool? Consumed
): IHaveSnowflakeId;