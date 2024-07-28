using System.Reflection;
using Microsoft.AspNetCore.Http.Metadata;

namespace VespionSoftworks.DIVA.Spine.Utilities.Results;

/// <summary>
/// An <see cref="IResult"/> that on execution will write an object to the response
/// with Spine Timeout (504) status code.
/// </summary>
public sealed class GatewayTimeout : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult
{
	/// <summary>
	/// Initializes a new instance of the <see cref="GatewayTimeout"/> class with the values.
	/// </summary>
	internal GatewayTimeout()
	{
	}

	/// <summary>
	/// Gets the HTTP status code: <see cref="StatusCodes.Status504GatewayTimeout"/>
	/// </summary>
	public int StatusCode => StatusCodes.Status504GatewayTimeout;

	int? IStatusCodeHttpResult.StatusCode => StatusCode;

	/// <inheritdoc/>
	public Task ExecuteAsync(HttpContext httpContext)
	{
		ArgumentNullException.ThrowIfNull(httpContext);

		// Creating the logger with a string to preserve the category after the refactoring.
		var loggerFactory = httpContext.RequestServices.GetRequiredService<ILoggerFactory>();
		var logger = loggerFactory.CreateLogger("VespionSoftworks.DIVA.Spine.Utilities.Results.GatewayTimeout");

		LogHelper.WritingResultAsStatusCode(logger, StatusCode);
		httpContext.Response.StatusCode = StatusCode;

		return Task.CompletedTask;
	}

	/// <inheritdoc/>
	static void IEndpointMetadataProvider.PopulateMetadata(MethodInfo method, EndpointBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(method);
		ArgumentNullException.ThrowIfNull(builder);

		builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status504GatewayTimeout, typeof(void)));
	}
}