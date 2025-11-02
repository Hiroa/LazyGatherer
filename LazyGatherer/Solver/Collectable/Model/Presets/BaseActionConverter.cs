using System;
using LazyGatherer.Solver.Collectable.Model.Actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static System.Enum;

namespace LazyGatherer.Solver.Collectable.Model.Presets;

public class BaseActionConverter : JsonConverter<BaseAction>
{
    private readonly StringEnumConverter stringEnumConverter = new();

    public override void WriteJson(JsonWriter writer, BaseAction? value, JsonSerializer serializer)
    {
        stringEnumConverter.WriteJson(writer, value?.ToPreset(), serializer);
    }

    public override BaseAction? ReadJson(
        JsonReader reader, Type objectType, BaseAction? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return existingValue;
        return TryParse(reader.Value.ToString(), out BaseAction.CollectableAction myStatus)
                   ? BaseAction.FromPreset(myStatus)
                   : existingValue;
    }
}
