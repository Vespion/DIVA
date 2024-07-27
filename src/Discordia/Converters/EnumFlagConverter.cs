using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VespionSoftworks.DIVA.Discordia.Converters;

public class EnumFlagConverter<T, TEnum> : JsonConverter<TEnum>
	where TEnum : struct, Enum
	where T: INumber<T>
{
	/// <inheritdoc />
	public override bool HandleNull => true;

	public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var value = reader.GetString();

		var enumValue = string.IsNullOrWhiteSpace(value) ? T.Zero : T.Parse(value, NumberStyles.Integer, null);

		return Unsafe.As<T, TEnum>(ref enumValue);
	}

	public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
	{
		switch (Type.GetTypeCode(typeof(TEnum)))
		{
			case TypeCode.Empty:
				writer.WriteNullValue();
				break;
			case TypeCode.Decimal:
				writer.WriteNumberValue(Unsafe.As<TEnum, decimal>(ref value));
				break;
			case TypeCode.Single:
				writer.WriteNumberValue(Unsafe.As<TEnum, float>(ref value));
				break;
			case TypeCode.Double:
				writer.WriteNumberValue(Unsafe.As<TEnum, double>(ref value));
				break;
			case TypeCode.Int32:
				writer.WriteNumberValue(Unsafe.As<TEnum, int>(ref value));
				break;
			case TypeCode.UInt32:
				writer.WriteNumberValue(Unsafe.As<TEnum, uint>(ref value));
				break;
			case TypeCode.Int64:
				writer.WriteNumberValue(Unsafe.As<TEnum, long>(ref value));
				break;
			case TypeCode.UInt64:
				writer.WriteNumberValue(Unsafe.As<TEnum, ulong>(ref value));
				break;
			default:
				writer.WriteStringValue(value.ToString());
				break;
		}
	}
}