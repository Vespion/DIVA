using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using VespionSoftworks.DIVA.Discordia.API.Models;
using Xunit.Abstractions;
using Xunit.Logging;
using LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory;

namespace VespionSoftworks.DIVA.Brain.Tests;

internal static class TestHelpers
{
	public static ILogger<T> CreateLogger<T>(ITestOutputHelper outputHelper)
	{
		var factory = LoggerFactory.Create(x =>
		{
			x.SetMinimumLevel(LogLevel.Trace);
			x.AddProvider(new LoggerProvider(outputHelper));
		});

		return factory.CreateLogger<T>();
	}

	public static HttpRequest AsHttpRequest(this object body, HeaderDictionary? headers = null, Dictionary<string, StringValues>? query = null)
	{
		return JsonConvert.SerializeObject(body).AsHttpRequest(headers, query);
	}

	public static HttpRequest AsHttpRequest(this string body, HeaderDictionary? headers = null, Dictionary<string, StringValues>? query = null)
	{
		return HttpRequestSetup(body, headers, query);
	}

	public static HttpRequest HttpRequestSetup(string body, HeaderDictionary? headers = null, Dictionary<string, StringValues>? query = null)
	{
		var httpContext = new DefaultHttpContext();
		// httpContext.Request.Method = "POST";
		// httpContext.Request.Scheme = "http";
		// httpContext.Request.Host = new HostString("localhost");
		// httpContext.Request.ContentType = "application/json";

		var stream = new MemoryStream();
		var writer = new StreamWriter(stream);
		writer.Write(body);
		writer.Flush();
		stream.Position = 0;
		httpContext.Request.Body = stream;
		httpContext.Request.ContentLength = stream.Length;
		httpContext.Request.ContentType = MediaTypeNames.Application.Json;

		if(query != null)
		{
			httpContext.Request.Query = new QueryCollection(query);
		}
		if (headers != null)
		{
			foreach (var (key, value) in headers)
			{
				httpContext.Request.Headers[key] = value;
			}
		}

		httpContext.RequestServices = new ServiceCollection()
			.Configure<JsonOptions>(x => x.SerializerOptions.TypeInfoResolverChain.Add(ModelJsonContext.Default))
			.BuildServiceProvider();

		return httpContext.Request;
	}
}