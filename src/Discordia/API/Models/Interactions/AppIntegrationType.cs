namespace VespionSoftworks.DIVA.Discordia.API.Models.Interactions;

/// <summary>
/// Defines where the application can be installed to.
/// </summary>
public enum AppIntegrationType: byte
{
	/// <summary>
	///     The application can be installed to a guild.
	/// </summary>
	GuildInstall = 0,

	/// <summary>
	///     The application can be installed to a user.
	/// </summary>
	UserInstall = 1
}