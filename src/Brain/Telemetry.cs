using System.Diagnostics;

namespace VespionSoftworks.DIVA.Brain;

internal static class Telemetry
{
	public static ActivitySource ActivitySource { get; } = new("VespionSoftworks.DIVA.Brain");
}