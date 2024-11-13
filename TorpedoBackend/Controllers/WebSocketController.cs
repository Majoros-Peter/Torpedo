using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using TorpedoCommon;

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
                        BaseMessage? message = JsonSerializer.Deserialize<BaseMessage>(json);
                        System.Diagnostics.Debug.WriteLine(message);
                    }
                }
            }
            else {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
