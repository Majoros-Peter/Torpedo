using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using TorpedoCommon;

namespace TorpedoClient
{
    class WebsocketClient
    {
        ClientWebSocket client;

        public async Task Connect()
        {
            client = new();
            await client.ConnectAsync(new Uri("ws://localhost:5118"), CancellationToken.None);
            await RecieveMessages();
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

                BaseMessage? message = JsonSerializer.Deserialize<BaseMessage>(json);
                if (message != null)
                {
                    MessageBox.Show(message.Type);
                }
            }
        }
    }
}
