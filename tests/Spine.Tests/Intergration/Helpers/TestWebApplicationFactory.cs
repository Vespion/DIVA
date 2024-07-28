using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VespionSoftworks.DIVA.Spine.Auth;
using VespionSoftworks.DIVA.Spine.Tests.Helpers;
using Xunit.Abstractions;
using Xunit.Logging;

namespace VespionSoftworks.DIVA.Spine.Tests.Intergration.Helpers;

public class TestWebApplicationFactory<TProgram>
	: WebApplicationFactory<TProgram> where TProgram : class
{
	internal ITestOutputHelper TestOutput { get; set; } = null!;

	internal bool PreserveHeaderValidation { get; set; }

	internal Action<HostBuilderContext,IConfigurationBuilder>? AppConfigurationHook { get; set; }

	protected override void ConfigureClient(HttpClient client)
	{
		client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("xUnit-Test")));

		base.ConfigureClient(client);
	}

	protected override IHost CreateHost(IHostBuilder builder)
	{
		builder.ConfigureLogging(lb =>
		{
			lb.SetMinimumLevel(LogLevel.Trace);
			lb.AddProvider(new LoggerProvider(TestOutput));
		});

		builder.ConfigureAppConfiguration(cb =>
		{
			cb.AddInMemoryCollection(new[]
			{
				new KeyValuePair<string, string?>("Authentication:Discord:Bot:ValidateHeaders", bool.FalseString)
			});
		});

		if(AppConfigurationHook != null)
		{
			builder.ConfigureAppConfiguration(AppConfigurationHook);
		}

		if (!PreserveHeaderValidation)
		{
			builder.ConfigureServices(services =>
			{
				var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDiscordHeaderValidator));

				if (descriptor != null)
				{
					services.Remove(descriptor);
				}

				services.AddScoped<IDiscordHeaderValidator, TestHeaderValidator>();
			});
		}

		return base.CreateHost(builder);
	}
}