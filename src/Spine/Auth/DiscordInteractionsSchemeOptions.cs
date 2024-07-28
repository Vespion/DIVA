using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication;

namespace VespionSoftworks.DIVA.Spine.Auth;

/// <summary>
/// Configuration options for the Discord Interactions authentication scheme.
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public class DiscordInteractionsSchemeOptions: AuthenticationSchemeOptions
{
	/// <summary>
	/// Public key used to verify the request signature.
	/// </summary>
	public string? PublicKey { get; set; }

	/// <summary>
	/// True if the request headers should be validated.
	/// </summary>
	public bool ValidateHeaders { get; set; }
}