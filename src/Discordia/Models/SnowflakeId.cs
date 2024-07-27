namespace VespionSoftworks.DIVA.Discordia.Models;

public readonly record struct SnowflakeId(ulong Id)
{
	private const long DiscordEpoch = 1420070400000;

	public DateTimeOffset Timestamp => DateTimeOffset.FromUnixTimeMilliseconds((long)((Id >> 22) + DiscordEpoch));

	/// <summary>
	/// Gets the uint64 representation of this snowflake ID.
	/// </summary>
	public static implicit operator ulong(SnowflakeId snowflakeId) => snowflakeId.Id;
}