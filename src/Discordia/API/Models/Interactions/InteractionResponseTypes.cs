namespace VespionSoftworks.DIVA.Discordia.API.Models.Interactions;

public enum InteractionResponseTypes: byte
{
	/// <summary>
	/// ACK a Ping
	/// </summary>
	Pong = 1,
	/// <summary>
	/// respond to an interaction with a message
	/// </summary>
	ChannelMessageWithSource = 4,
	/// <summary>
	/// ACK an interaction and edit a response later, the user sees a loading state
	/// </summary>
	DeferredChannelMessageWithSource = 5,
	/// <summary>
	/// for components, ACK an interaction and edit the original message later; the user does not see a loading state
	/// </summary>
	DeferredUpdateMessage = 6,
	/// <summary>
	/// for components, edit the message the component was attached to
	/// </summary>
	UpdateMessage = 7,
	/// <summary>
	/// respond to an autocomplete interaction with suggested choices
	/// </summary>
	ApplicationCommandAutocompleteResult = 8,
	/// <summary>
	/// respond to an interaction with a popup modal
	/// </summary>
	Modal = 9,
	/// <summary>
	/// Deprecated; respond to an interaction with an upgrade button, only available for apps with monetization enabled
	/// </summary>
	[Obsolete("Deprecated; respond to an interaction with an upgrade button, only available for apps with monetization enabled")]
	PremiumRequired = 10
}