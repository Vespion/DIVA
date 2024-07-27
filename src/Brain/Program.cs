using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VespionSoftworks.DIVA.Discordia.API.Models;
using Configuration = System.Configuration.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((ctx, services) => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Add(ModelJsonContext.Default));

        services.AddOptions();
        services.Configure<Configuration>(ctx.Configuration);
    })
    .Build();

host.Run();
