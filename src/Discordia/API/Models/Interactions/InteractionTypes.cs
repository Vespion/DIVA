namespace VespionSoftworks.DIVA.Discordia.API.Models.Interactions;

public enum InteractionTypes : byte
{
	Ping = 1,
	ApplicationCommand = 2,
	MessageComponent = 3,
	ApplicationCommandAutocomplete = 4,
	ModelSubmit = 5
}