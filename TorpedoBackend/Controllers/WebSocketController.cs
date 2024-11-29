using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using TorpedoCommon;
using TorpedoCommon.MessageTypes;

namespace TorpedoBackend.Controllers
{
    public class WebSocketController : Controller
    {
        [Route("/")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var websocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                var buffer = new byte[1024 * 4];
                while (websocket.State == WebSocketState.Open) {
                    var result = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        var options = new JsonSerializerOptions();
                        options.Converters.Add(new MessageConverter());
                        BaseMessage? message = JsonSerializer.Deserialize<BaseMessage>(json, options);

                        await SendMessage(websocket, new AuthResponse() { Success = true });
                    }
                }
            }
            else {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        static async Task SendMessage(WebSocket socket, BaseMessage message) {
            await socket.SendAsync(message.ToArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
