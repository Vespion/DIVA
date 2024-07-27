using System.Reflection;
using Microsoft.AspNetCore.Http.Metadata;

namespace VespionSoftworks.DIVA.Gateway.Utilities.Results;

/// <summary>
/// An <see cref="IResult"/> that on execution will write an object to the response
/// with Bad Gateway (502) status code.
/// </summary>
public sealed class BadGateway : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult
{
	/// <summary>
	/// Initializes a new instance of the <see cref="BadGateway"/> class with the values.
	/// </summary>
	internal BadGateway()
	{
	}

	/// <summary>
	/// Gets the HTTP status code: <see cref="StatusCodes.Status504GatewayTimeout"/>
	/// </summary>
	public int StatusCode => StatusCodes.Status502BadGateway;

	int? IStatusCodeHttpResult.StatusCode => StatusCode;

	/// <inheritdoc/>
	public Task ExecuteAsync(HttpContext httpContext)
	{
		ArgumentNullException.ThrowIfNull(httpContext);

		// Creating the logger with a string to preserve the category after the refactoring.
		var loggerFactory = httpContext.RequestServices.GetRequiredService<ILoggerFactory>();
		var logger = loggerFactory.CreateLogger("VespionSoftworks.DIVA.Gateway.Utilities.Results.BadGateway");

		LogHelper.WritingResultAsStatusCode(logger, StatusCode);
		httpContext.Response.StatusCode = StatusCode;

		return Task.CompletedTask;
	}

	/// <inheritdoc/>
	static void IEndpointMetadataProvider.PopulateMetadata(MethodInfo method, EndpointBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(method);
		ArgumentNullException.ThrowIfNull(builder);

		builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status502BadGateway, typeof(void)));
	}
}