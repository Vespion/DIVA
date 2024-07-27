using Bogus;
using VespionSoftworks.DIVA.Discordia.Models;

namespace VespionSoftworks.DIVA.Discordia.Tests;

public class SnowflakeIdTests
{
	public static TheoryData<ulong, string> SnowflakeGenerator()
	{
		var faker = new Faker()
		{
			Random = new Randomizer(15052001)
		};

		var data = new TheoryData<ulong, string>
		{
			{ 175928847299117063, "2016-04-30 11:18:25.796Z" },
			{ Convert(DateTimeOffset.Parse("2016-04-30 11:18:25.796Z")), "2016-04-30 11:18:25.796Z" }
		};

		for (var i = 0; i < 50; i++)
		{
			var (id, timestamp) = Generate();
			data.Add(id, timestamp);
		}

		return data;

		ulong Convert(DateTimeOffset timestamp)
		{
			var unixTimeMilliseconds = timestamp.ToUnixTimeMilliseconds();
			var snowflakeId = (unixTimeMilliseconds - 1420070400000) << 22;
			snowflakeId |= 00001 << 21;
			snowflakeId |= 00001 << 16;
			snowflakeId |= 000000000111 << 11;
			return (ulong)snowflakeId;
		}

		(ulong, string) Generate()
		{
			var timestamp = faker.Date.BetweenOffset(
				faker.Date.PastOffset(3),
				faker.Date.FutureOffset(3)
			);

			var id = Convert(timestamp);

			return (id, timestamp.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fffZ"));
		}
	}

	[Theory]
	[MemberData(nameof(SnowflakeGenerator))]
	public void ParsesIdCorrectly(ulong id, string expectedTimestampString)
	{
		var expectedTimestamp = DateTimeOffset.Parse(expectedTimestampString);

		var snowflakeId = new SnowflakeId(id);

		Assert.Equal(expectedTimestamp, snowflakeId.Timestamp);
	}
}