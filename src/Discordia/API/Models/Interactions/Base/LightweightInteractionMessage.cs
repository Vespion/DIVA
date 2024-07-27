using Discordia.Models;
using VespionSoftworks.DIVA.Discordia.Models;

namespace VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Base;

public readonly record struct LightweightInteractionMessage(SnowflakeId Id, InteractionTypes Type) : IHaveSnowflakeId;