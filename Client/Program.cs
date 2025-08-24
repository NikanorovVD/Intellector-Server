using Microsoft.AspNetCore.SignalR.Client;
using Shared.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Client
{
    public class CreateOpenLobbyRequest
    {
        public TimeControlDto? TimeControl { get; set; }
        public ColorChoice ColorChoice { get; set; }
        public bool Rating { get; set; }
    }
    public class TokenDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
    public class StartGameMessage
    {
        public string GameId { get; set; }
        public string WhitePlayerId { get; set; }
        public string BlackPlayerId { get; set; }
        public TimeControlDto? TimeControl { get; set; }
        public bool Rating { get; set; }
    }
    public class OpenLobbyDto
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public TimeControlDto? TimeControl { get; set; }
        public ColorChoice ColorChoice { get; set; }
        public bool Rating { get; set; }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                bool gameStart = false;

                // запрос токена
                Console.Write("enter username: ");
                string name = Console.ReadLine();
                Console.Write("enter password: ");
                string password = Console.ReadLine();
                var httpClient = new HttpClient() { BaseAddress = new Uri("https://localhost:7053/") };
                var request = new HttpRequestMessage(HttpMethod.Post, "Login")
                {
                    Method = HttpMethod.Post,
                    Content = JsonContent.Create(new AuthRequest
                    {
                        UserName = name,
                        Password = password
                    })
                };
                HttpResponseMessage response = await httpClient.SendAsync(request);
                Console.WriteLine("auth: "  + response.StatusCode);
                TokenDto tokenDto = JsonSerializer.Deserialize<TokenDto>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions.Web);
                string token = tokenDto.Token;

                // установка авторизации
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var hubConnection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:7055/game", options => options.AccessTokenProvider = () => Task.FromResult(token))
                    .Build();

                await hubConnection.StartAsync();

                httpClient = new HttpClient() { BaseAddress = new Uri("https://localhost:7054") };
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                //команды
                while (true)
                {
                    Console.Write("enter command: ");
                    string command = Console.ReadLine();
                    switch (command)
                    {
                        case "list":
                            {
                                request = new HttpRequestMessage(HttpMethod.Get, "MatchMaking/open");
                                response = await httpClient.SendAsync(request);
                                IEnumerable<OpenLobbyDto> lobbies = JsonSerializer.Deserialize<IEnumerable<OpenLobbyDto>>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions.Web);
                                foreach (OpenLobbyDto lobby in lobbies)
                                {
                                    Console.WriteLine($"{lobby.Id} - {lobby.OwnerId}");
                                }
                                if (!lobbies.Any())
                                {
                                    Console.WriteLine("Empty");
                                }
                                break;
                            }
                        case "create":
                            {
                                request = new HttpRequestMessage(HttpMethod.Post, "MatchMaking/open")
                                {
                                    Content = JsonContent.Create(new CreateOpenLobbyRequest
                                    {
                                        ColorChoice = ColorChoice.White,
                                        Rating = false,
                                        TimeControl = null,
                                    })
                                };
                                response = await httpClient.SendAsync(request);
                                Console.WriteLine("create: " + response.StatusCode);
                                string content = await response.Content.ReadAsStringAsync();
                                int id = JsonSerializer.Deserialize<int>(content, JsonSerializerOptions.Web);
                                Console.WriteLine($"created with id {id}");

                                StartGameMessage gameInfo = null;
                                hubConnection.On<StartGameMessage>("ReceiveStartGame", gi =>
                                {
                                    gameInfo = gi;
                                    gameStart = true;
                                });

                                while (!gameStart)
                                { }

                                Console.WriteLine($"game info: id={gameInfo.GameId} black={gameInfo.BlackPlayerId} white={gameInfo.WhitePlayerId}");
                                await EnterGame(gameInfo.GameId);
                                break;
                            }
                        case "join":
                            {
                                Console.Write("enter id: ");
                                int id = int.Parse(Console.ReadLine());

                                request = new HttpRequestMessage(HttpMethod.Post, $"MatchMaking/open/join/{id}");
                                response = await httpClient.SendAsync(request);

                                StartGameMessage gameInfo = null;
                                hubConnection.On<StartGameMessage>("ReceiveStartGame", gi =>
                                {
                                    gameInfo = gi;
                                    gameStart = true;
                                });

                                while (!gameStart)
                                { }

                                Console.WriteLine($"game info: id={gameInfo.GameId} black={gameInfo.BlackPlayerId} white={gameInfo.WhitePlayerId}");
                                await EnterGame(gameInfo.GameId);
                                break;
                            }
                        default:
                            Console.WriteLine("Unknown command");
                            break;
                    }
                }

                async Task EnterGame(string gameId)
                {
                    hubConnection.On<Move, PlayerColor>("ReceiveMove", (move, color) =>
                    {
                        Console.WriteLine($"[{color}]: {move}");
                    });

                    while (true)
                    {
                        string? mes = Console.ReadLine();
                        if (mes != null)
                        {
                            await hubConnection.InvokeAsync("SendMove", gameId, new Move(1, 1, 2, 2, (PieceType)int.Parse(mes)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                await Task.Delay(300000);
                Console.ReadKey();
            }
        }
    }
}
