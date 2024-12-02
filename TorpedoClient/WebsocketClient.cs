﻿using System;
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
    class WebsocketClient
    {
        ClientWebSocket client;

        public event Action<List<string>> onPlayerListRecieved;
        public event Action<string> onActionFailed;
        public event Action<GameStateUpdate> onGameStateUpdated;

        public async Task Connect()
        {
            client = new();
            await client.ConnectAsync(new Uri("ws://localhost:5118"), CancellationToken.None);
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
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
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
                    default:
                        break;
                }
            }
        }
    }
}