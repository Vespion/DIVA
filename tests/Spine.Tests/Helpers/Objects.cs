using System.Net.Mime;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using VespionSoftworks.DIVA.Discordia.API.Models;

namespace VespionSoftworks.DIVA.Spine.Tests.Helpers;

internal static class Objects
{
	public static TelemetryClient InitializeMockTelemetryChannel()
	{
		// Application Insights TelemetryClient doesn't have an interface (and is sealed)
		// Spin -up our own homebrew mock object
		var mockTelemetryChannel = new MockTelemetryChannel();
		var mockTelemetryConfig = new TelemetryConfiguration
		{
			TelemetryChannel = mockTelemetryChannel,
			InstrumentationKey = Guid.NewGuid().ToString(),
		};

		var mockTelemetryClient = new TelemetryClient(mockTelemetryConfig);
		return mockTelemetryClient;
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
			.Configure<JsonOptions>(x => x.SerializerOptions.TypeInfoResolverChain.Add(GatewayModelJsonContext.Default))
			.BuildServiceProvider();

		return httpContext.Request;
	}
}