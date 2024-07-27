using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace VespionSoftworks.DIVA.Gateway.Auth;

internal static class AuthenticationExtensions
{
	internal const string DiscordInteractionsScheme = "discord_interactions";

	public static AuthenticationBuilder AddDiscordInteractions(this AuthenticationBuilder builder, string schemeName = DiscordInteractionsScheme, Action<DiscordInteractionsSchemeOptions>? configureOptions = null)
	{
		builder.Services.TryAddScoped<IDiscordHeaderValidator, DiscordHeaderValidator>();
		builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<DiscordInteractionsSchemeOptions>, DiscordInteractionsSchemeConfigureOptions>());

		return builder.AddScheme<DiscordInteractionsSchemeOptions, DiscordInteractionsSchemeHandler>(
			schemeName,
			"Discord Interactions",
			configureOptions
		);
	}

	public static AuthorizationPolicyBuilder AddDiscordAuthenticationScheme(this AuthorizationPolicyBuilder builder) => builder.AddAuthenticationSchemes(DiscordInteractionsScheme);
}