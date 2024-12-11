using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using TorpedoCommon;
using TorpedoCommon.MessageTypes;

namespace TorpedoClient
{
    public class WebsocketClient
    {
        ClientWebSocket client;
        public string Username { get; set; }

        public event Action<List<string>> onPlayerListRecieved;
        public event Action<string> onActionFailed;
        public event Action<GameStateUpdate> onGameStateUpdated;
        public event Action<string> onGameOver;

        public async Task Connect(string username)
        {
            client = new();
            await client.ConnectAsync(new Uri("ws://localhost:5118"), CancellationToken.None);
            this.Username = username;
            await SendMessage(new LoginRequest() { Username = username });
            RecieveMessages();
        }

        public async Task SendMessage(BaseMessage message)
        {
            await client.SendAsync(message.ToArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task RecieveMessages()
        {
            while (client.State == WebSocketState.Open)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
                WebSocketReceiveResult result = await client.ReceiveAsync(buffer, CancellationToken.None);
                string json = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);

                var options = new JsonSerializerOptions();
                options.Converters.Add(new MessageConverter());
                BaseMessage message = JsonSerializer.Deserialize<BaseMessage>(json, options)!;

                switch (message.Type)
                {
                    case "PlayerListResponse":
                        PlayerListResponse playerListResponse = message as PlayerListResponse;
                        onPlayerListRecieved(playerListResponse.players);
                        break;
                    case "FailedResponse":
                        FailedResponse failedResponse = message as FailedResponse;
                        onActionFailed(failedResponse.Message);
                        break;
                    case "GameStateUpdate":
                        GameStateUpdate update = message as GameStateUpdate;
                        onGameStateUpdated(update);
                        break;
                    case "GameOver":
                        GameOver gameOver = message as GameOver;
                        onGameOver(gameOver.Winner);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
