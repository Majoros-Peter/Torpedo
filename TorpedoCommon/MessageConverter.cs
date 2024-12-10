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

                switch (type)
                {
                    case "LoginRequest":
                        return JsonSerializer.Deserialize<LoginRequest>(root.GetRawText());
                    case "PlayerListResponse":
                        return JsonSerializer.Deserialize<PlayerListResponse>(root.GetRawText());
                    case "FailedResponse":
                        return JsonSerializer.Deserialize<FailedResponse>(root.GetRawText());
                    case "StartGameMessage":
                        return JsonSerializer.Deserialize<StartGameMessage>(root.GetRawText());
                    case "GameStateUpdate":
                        return JsonSerializer.Deserialize<GameStateUpdate>(root.GetRawText());
                    case "PlaceShipsMessage":
                        return JsonSerializer.Deserialize<PlaceShipsMessage>(root.GetRawText());
                    case "ShootMessage":
                        return JsonSerializer.Deserialize<ShootMessage>(root.GetRawText());
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
