using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VespionSoftworks.DIVA.Discordia.Converters;

public class CultureLanguageCodeConverter : JsonConverter<CultureInfo>
{
	/// <inheritdoc />
	public override bool HandleNull => true;

	public override CultureInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var cultureCode = reader.GetString();

		return string.IsNullOrWhiteSpace(cultureCode) ? null : new CultureInfo(cultureCode);
	}

	public override void Write(Utf8JsonWriter writer, CultureInfo? value, JsonSerializerOptions options)
	{
		if (value == null)
		{
			writer.WriteNullValue();
		}
		else
		{
			writer.WriteStringValue(value.Name);
		}
	}
}