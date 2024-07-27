using System.Text.Json;
using System.Text.Json.Serialization;
using VespionSoftworks.DIVA.Discordia.Models;

namespace VespionSoftworks.DIVA.Discordia.Converters;

public class NullableSnowflakeConverter: JsonConverter<SnowflakeId?>
{
	/// <inheritdoc />
	public override bool HandleNull => true;

	/// <inheritdoc />
	public override SnowflakeId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var value = reader.GetString();
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}

		var id = ulong.Parse(value);
		return new SnowflakeId(id);
	}

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, SnowflakeId? value, JsonSerializerOptions options)
	{
		if (value.HasValue)
		{
			writer.WriteStringValue(value.Value.Id.ToString());
		}
		else
		{
			writer.WriteNullValue();
		}
	}
}

public class SnowflakeConverter: JsonConverter<SnowflakeId>
{
	/// <inheritdoc />
	public override bool HandleNull => false;

	/// <inheritdoc />
	public override SnowflakeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var value = reader.GetString();
		if (value is null)
		{
			throw new JsonException();
		}

		var id = ulong.Parse(value);
		return new SnowflakeId(id);
	}

	/// <inheritdoc />
	public override void Write(Utf8JsonWriter writer, SnowflakeId value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.Id.ToString());
	}
}