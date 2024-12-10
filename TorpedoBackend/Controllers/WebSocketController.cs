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
        public static Random random = new();
        public static Dictionary<string, WebSocket> players = new();
        public static List<Game> games = new();


        [Route("/")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var websocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                try
                {
                    var buffer = new byte[1024 * 4];
                    while (websocket.State == WebSocketState.Open)
                    {
                        var result = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            string json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                            var options = new JsonSerializerOptions();
                            options.Converters.Add(new MessageConverter());
                            BaseMessage message = JsonSerializer.Deserialize<BaseMessage>(json, options)!;

                            switch (message.Type)
                            {
                                case "LoginRequest":
                                    LoginRequest loginRequest = message as LoginRequest;

                                    if (players.ContainsKey(loginRequest.Username)) {
                                        await SendMessage(websocket, new FailedResponse() { Message = "This username is taken!" });
                                        break;
                                    }

                                    players[loginRequest.Username] = websocket;
                                    await BroadcastMessage(new PlayerListResponse() { players = [.. players.Keys] });
                                    break;
                                case "StartGameMessage":
                                    StartGameMessage startGameMessage = message as StartGameMessage;

                                    if (games.Any(x => x.Player1Name == startGameMessage.Player2Name || x.Player2Name == startGameMessage.Player2Name))
                                    {
                                        await SendMessage(websocket, new FailedResponse() { Message = "Player is already in a game!" });
                                        break;
                                    }

                                    Game newGame = new Game() { Id = random.Next(),
                                        Player1Name = players.Where(pair => pair.Value == websocket).First().Key,
                                        Player2Name = startGameMessage.Player2Name };
                                    games.Add(newGame);

                                    await SendToPlayers(newGame, new GameStateUpdate() { GameState = newGame });
                                    break;
                                case "PlaceShipsMessage":
                                    PlaceShipsMessage placeShipsMessage = message as PlaceShipsMessage;
                                    Game placeShipsGame = games.Find(x => x.Id == placeShipsMessage.GameId)!;

                                    if (placeShipsGame.Player1Name == players.Where(pair => pair.Value == websocket).First().Key)
                                    {
                                        placeShipsGame.Player1Ships = placeShipsMessage.Ships;
                                    }
                                    else
                                    {
                                        placeShipsGame.Player2Ships = placeShipsMessage.Ships;
                                    }


                                    if (placeShipsGame.Player1Ships != null && placeShipsGame.Player2Ships != null)
                                    {
                                        placeShipsGame.SetupPhase = false;
                                        await SendToPlayers(placeShipsGame, new GameStateUpdate { GameState = placeShipsGame });
                                    }
                                    break;
                                case "ShootMessage":
                                    ShootMessage shootMessage = message as ShootMessage;
                                    Game shootGame = games.Find(x => x.Id == shootMessage.GameId)!;

                                    if (shootGame.Player1Name == players.Where(pair => pair.Value == websocket).First().Key)
                                    {
                                        shootGame.Player1Shots[shootMessage.X * 1 + shootMessage.Y] = true;
                                        shootGame.isPlayer1Next = false;
                                    }
                                    else {
                                        shootGame.Player2Shots[shootMessage.X * 1 + shootMessage.Y] = true;
                                        shootGame.isPlayer1Next = true;
                                    }
                                    
                                    await SendToPlayers(shootGame, new GameStateUpdate { GameState = shootGame });
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                catch (WebSocketException)
                {
                    players.Remove(players.Where(pair => pair.Value == websocket).First().Key);
                    await BroadcastMessage(new PlayerListResponse() { players = [.. players.Keys] });
                }
            }
            else {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        static async Task SendMessage(WebSocket socket, BaseMessage message) {
            await socket.SendAsync(message.ToArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        static async Task BroadcastMessage(BaseMessage message) {
            foreach (WebSocket socket in players.Values)
            {
                await SendMessage(socket, message);
            }
        }

        static async Task SendToPlayers(Game game, BaseMessage message)
        {
            await SendMessage(players[game.Player1Name], message);
            await SendMessage(players[game.Player2Name], message);
        }
    }
}
