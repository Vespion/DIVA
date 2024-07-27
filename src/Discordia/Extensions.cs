using System.Runtime.CompilerServices;

namespace VespionSoftworks.DIVA.Discordia;

internal static class Extensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool TryConvert<TEnum, T>(this TEnum @enum, out T val)
		where TEnum : struct, Enum
		where T : struct, IConvertible, IFormattable, IComparable
	{
		if (Unsafe.SizeOf<T>() == Unsafe.SizeOf<TEnum>())
		{
			val = Unsafe.As<TEnum, T>(ref @enum);
			return true;
		}
		val = default;
		return false;
	}
}