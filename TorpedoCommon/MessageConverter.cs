using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TorpedoCommon.MessageTypes;

namespace TorpedoCommon
{
    public class MessageConverter : JsonConverter<BaseMessage>
    {
        public override BaseMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;
                var type = root.GetProperty("Type").GetString();

                Type asd = Type.GetType($"TorpedoCommon.{type}");

                switch (type)
                {
                    case "LoginRequest":
                        return JsonSerializer.Deserialize<LoginRequest>(root.GetRawText());
                    case "AuthResponse":
                        return JsonSerializer.Deserialize<AuthResponse>(root.GetRawText());
                    default:
                        throw new JsonException("Unknown message type");
                }
            }
        }

        public override void Write(Utf8JsonWriter writer, BaseMessage value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
