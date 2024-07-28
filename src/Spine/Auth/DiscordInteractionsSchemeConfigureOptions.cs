using Microsoft.Extensions.Options;

namespace VespionSoftworks.DIVA.Spine.Auth;

internal class DiscordInteractionsSchemeConfigureOptions(IConfiguration configuration): IConfigureNamedOptions<DiscordInteractionsSchemeOptions>
{
	public void Configure(DiscordInteractionsSchemeOptions options)
	{
		configuration.Bind("Authentication:Discord:Bot", options);
	}

	public void Configure(string? schemeName, DiscordInteractionsSchemeOptions options)
	{
		if (schemeName is null)
		{
			return;
		}

		configuration.Bind("Authentication:Discord:Bot", options);
	}
}