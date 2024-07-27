using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using VespionSoftworks.DIVA.Discordia.API.Models.Interactions.Base;
using VespionSoftworks.DIVA.Discordia.Converters;
using VespionSoftworks.DIVA.Discordia.Models;

namespace VespionSoftworks.DIVA.Discordia.API.Models;

/// <summary>
/// Provides a context for serialization and deserialization of JSON API data.
/// </summary>
[JsonSourceGenerationOptions(
	JsonSerializerDefaults.Web,
	GenerationMode = JsonSourceGenerationMode.Default,
	Converters = [
		// typeof(JsonStringEnumConverter<AppIntegrationType>),
		// typeof(JsonStringEnumConverter<PermissionFlags>),
		// typeof(JsonStringEnumConverter<EntitlementTypes>),
		// typeof(JsonStringEnumConverter<AppIntegrationType>),
		typeof(EnumFlagConverter<ulong, PermissionFlags>),
		typeof(SnowflakeConverter),
		typeof(NullableSnowflakeConverter),
		typeof(CultureLanguageCodeConverter)
	],
	UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
	NumberHandling = JsonNumberHandling.AllowReadingFromString,
	PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
	PropertyNameCaseInsensitive = true,
	PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace
)]
[JsonSerializable(typeof(IInteractionsMessage))]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public partial class ModelJsonContext: JsonSerializerContext;