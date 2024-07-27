using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace VespionSoftworks.DIVA.Brain.Functions;

/// <summary>
/// Base class for all functions in the project
/// </summary>
/// <remarks>
/// This class is used to provide a common base for all functions in the project. It provides common functionality such as logging and validation of headers.
/// </remarks>
/// <param name="logger">The concrete logger for the function</param>
/// <typeparam name="T">The concrete function type</typeparam>
public abstract class FunctionBase<T>(ILogger<T> logger)
	where T : FunctionBase<T>
{
	/// <summary>
	/// Used to define the HTTP methods that are supported by the function
	/// </summary>
	protected class HttpMethods
	{
		/// <summary>
		/// HTTP GET method
		/// </summary>
		public const string Get = "get";

		/// <summary>
		/// Http POST method
		/// </summary>
		public const string Post = "post";
	}

	/// <summary>
	/// Logger instance for the function
	/// </summary>
	protected readonly ILogger<T> Logger = logger;

	public abstract Task<IActionResult> Run(HttpRequest req);
}