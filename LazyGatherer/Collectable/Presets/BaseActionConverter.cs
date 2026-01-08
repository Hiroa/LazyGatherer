using System;
using LazyGatherer.Collectable.Actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static System.Enum;

namespace LazyGatherer.Collectable.Presets;

public class BaseActionConverter : JsonConverter<CollectableAction>
{
    private readonly StringEnumConverter stringEnumConverter = new();

    public override void WriteJson(JsonWriter writer, CollectableAction? value, JsonSerializer serializer)
    {
        stringEnumConverter.WriteJson(writer, value?.ToPreset(), serializer);
    }

    public override CollectableAction? ReadJson(
        JsonReader reader, Type objectType, CollectableAction? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.Value == null)
            return existingValue;
        return TryParse(reader.Value.ToString(), out CollectableAction.Names myStatus)
                   ? CollectableAction.FromPreset(myStatus)
                   : existingValue;
    }
}
