using System.Text.Json.Serialization;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Ping;

namespace VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Base;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(typeof(PingResponse), (int)InteractionResponseTypes.Pong)]
public interface IInteractionResponse
{

}

public interface IInteractionResponse<out T>: IInteractionResponse
{
	[JsonPropertyName("data")]
	public T? Data { get; }
}