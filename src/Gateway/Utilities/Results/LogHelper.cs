namespace VespionSoftworks.DIVA.Gateway.Utilities.Results;

internal static partial class LogHelper
{
	[LoggerMessage(1, LogLevel.Information,
		"Setting HTTP status code {StatusCode}.",
		EventName = "WritingResultAsStatusCode")]
	public static partial void WritingResultAsStatusCode(ILogger logger, int statusCode);
}