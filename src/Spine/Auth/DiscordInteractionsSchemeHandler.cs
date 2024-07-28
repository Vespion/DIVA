using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace VespionSoftworks.DIVA.Spine.Auth;

internal class DiscordInteractionsSchemeHandler(
	IOptionsMonitor<DiscordInteractionsSchemeOptions> options,
	ILoggerFactory logger,
	UrlEncoder encoder,
	TelemetryClient telemetry,
	IDiscordHeaderValidator validator
	)
	: AuthenticationHandler<DiscordInteractionsSchemeOptions>(options, logger, encoder)
{
	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		using var operation = telemetry.StartOperation<DependencyTelemetry>(nameof(HandleAuthenticateAsync));
		operation.Telemetry.Type = "Internal/Authentication";
		operation.Telemetry.Properties["Scheme"] = Scheme.Name;
		operation.Telemetry.Properties["ValidateHeaders"] = Options.ValidateHeaders.ToString();
		operation.Telemetry.Properties["PublicKey"] = Options.PublicKey;

		if (Options.ValidateHeaders)
		{
			var result = await validator.ValidateHeaders(Request, Options.PublicKey);
			operation.Telemetry.Success = result.IsSuccess;

			if (result.IsFailed)
			{
				return AuthenticateResult.Fail(result.Errors.First().ToString() ?? string.Empty);
			}
		}

		var identity = new GenericIdentity(Request.Headers.UserAgent.ToString(), Scheme.Name);
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, Scheme.Name);

		operation.Telemetry.Success = true;
		return AuthenticateResult.Success(ticket);
	}




}