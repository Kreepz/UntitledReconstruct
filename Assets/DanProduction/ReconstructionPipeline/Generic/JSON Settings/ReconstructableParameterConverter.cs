using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ReconstructableParameterConverter : JsonConverter<ReconstructableParameter>
{
    public override void WriteJson(JsonWriter writer, ReconstructableParameter value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        
        writer.WritePropertyName("ID");
        writer.WriteValue(value.ID);
        
        switch (value)
        {
            case ReconstructableString stringParameter:
                writer.WritePropertyName("Type");
                writer.WriteValue("String");
            
                writer.WritePropertyName("Value");
                writer.WriteValue(stringParameter.Value);
                break;
            case ReconstructableBool boolParameter:
                writer.WritePropertyName("Type");
                writer.WriteValue("Bool");
            
                writer.WritePropertyName("Value");
                writer.WriteValue(boolParameter.Value);
                break;
            case ReconstructableInt intParameter:
                writer.WritePropertyName("Type");
                writer.WriteValue("Int");
            
                writer.WritePropertyName("Value");
                writer.WriteValue(intParameter.Value);
                break;
            case ReconstructableFloat floatParameter:
                writer.WritePropertyName("Type");
                writer.WriteValue("Float");
            
                writer.WritePropertyName("Value");
                writer.WriteValue(floatParameter.Value);
                break;
        }
        
        writer.WriteEndObject();
    }

    public override ReconstructableParameter ReadJson(JsonReader reader, Type objectType, ReconstructableParameter existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);

        string id = jsonObject["ID"]?.ToObject<string>();
        string type = jsonObject["Type"]?.ToObject<string>();

        switch (type)
        {
            case "String":
            {
                ReconstructableString parameter = new()
                {
                    ID = id
                };
                parameter.SetStringValue(
                    jsonObject["Value"]?.ToObject<string>());

                return parameter; 
            }
            case "Bool":
            {
                ReconstructableBool parameter = new()
                {
                    ID = id
                };
                parameter.SetBoolValue(
                    jsonObject["Value"]?.ToObject<bool>());
                return parameter;
            }

            case "Int":
            {
                ReconstructableInt parameter = new()
                {
                    ID = id
                };
                parameter.SetValue(
                    jsonObject["Value"]?.ToObject<int>());
                return parameter;
            }

            case "Float":
            {
                ReconstructableFloat parameter = new()
                {
                    ID = id
                };
                parameter.SetValue(
                    jsonObject["Value"]?.ToObject<float>());
                
                return parameter;
            }
            default: 
                throw new JsonSerializationException(
                    $"Unknown reconstructable parameter type: {type}");
        }
    }
}
