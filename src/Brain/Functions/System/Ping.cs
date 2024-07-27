using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Ping;

namespace VespionSoftworks.DIVA.Brain.Functions.System;

public class Ping : FunctionBase<Ping>
{
	public Ping(ILogger<Ping> logger) : base(logger)
	{
	}

	[Function("Ping")]
	public override async Task<IActionResult> Run(
		[HttpTrigger(
			AuthorizationLevel.Anonymous,
			HttpMethods.Post,
			Route = "system/ping")
		] HttpRequest req
	)
	{
		using var activity = Telemetry.ActivitySource.StartActivity();
		Logger.LogDebug("Starting request processing");

		Logger.LogDebug("Reading request body as {Type}", typeof(PingRequest));
		var data = await req.ReadFromJsonAsync<PingRequest>();

		Logger.LogInformation("C# HTTP trigger function processed a request.");
		return new OkObjectResult("Welcome to Azure Functions!");
	}
}