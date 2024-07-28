using System.Diagnostics.CodeAnalysis;
using VespionSoftworks.DIVA.Discordia.API.Models;
using VespionSoftworks.DIVA.Spine.Auth;
using VespionSoftworks.DIVA.Spine.Endpoints;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddConsole();
builder.Services.AddApplicationInsightsTelemetry();


builder.Services.AddAuthentication()
	.AddDiscordInteractions();

builder.Services.AddAuthorization();

builder.Services.ConfigureHttpJsonOptions(options =>
{
	foreach (var converter in GatewayModelJsonContext.Default.Options.Converters)
	{
		if(options.SerializerOptions.Converters.Any(x => x.GetType() == converter.GetType()))
		{
			continue;
		}

		options.SerializerOptions.Converters.Add(converter);
	}

    options.SerializerOptions.TypeInfoResolverChain.Add(GatewayModelJsonContext.Default);
    // options.SerializerOptions.TypeInfoResolverChain.Add(ModelJsonContext.Default);
});

var app = builder.Build();

app.UseAuthorization();

app.RegisterInteractionEndpoints();
app.Run();

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable once MissingXmlDoc
namespace VespionSoftworks.DIVA.Spine
{
	[ExcludeFromCodeCoverage]
	public partial class Program
	{ }
}