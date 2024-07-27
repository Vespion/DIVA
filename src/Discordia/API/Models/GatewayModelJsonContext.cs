using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Base;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Ping;
using VespionSoftworks.DIVA.Discordia.Converters;

namespace VespionSoftworks.DIVA.Discordia.API.Models;

/// <summary>
/// Provides a context for serialization and deserialization of JSON API data.
/// </summary>
/// <remarks>
/// This context is specifically designed for use by the gateway and is thus highly stripped down.
/// </remarks>
[JsonSourceGenerationOptions(
	JsonSerializerDefaults.Web,
	GenerationMode = JsonSourceGenerationMode.Default,
	Converters = [
		typeof(SnowflakeConverter)
	],
	UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
	NumberHandling = JsonNumberHandling.AllowReadingFromString,
	PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
	PropertyNameCaseInsensitive = true,
	PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace
)]
[JsonSerializable(typeof(LightweightInteractionMessage))]
[JsonSerializable(typeof(PingResponse))]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public partial class GatewayModelJsonContext: JsonSerializerContext;