using Domain.Entities;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Domain.Service.Services.ServiceApiExternal
{
    public class ConnectClientService
    {
        private readonly HttpClient _httpClient;

        public ConnectClientService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://127.0.0.1:8000/")
            };
        }
        public async Task<string> ConnectAndSend(Question question)
        {
            try
            {
                using ClientWebSocket ws = new ClientWebSocket();
                Uri serverUri = new Uri("ws://127.0.0.1:8000/ws");
                await ws.ConnectAsync(serverUri, CancellationToken.None);
                if (question == null)
                {
                    throw new Exception("Pergunta não pode ser vazia.");
                }
                Console.WriteLine("Tipo do usuário : " + question.user_type);
                if (ws.State == WebSocketState.Open)
                {
                    if (string.IsNullOrEmpty(question.session_id) || !Guid.TryParse(question.session_id, out _))
                    {
                        question.session_id = Guid.NewGuid().ToString();
                    }

                    await SendMessage(ws, question);
                    Console.WriteLine("Pergunta : " + question.text);
                    return await ReceiveMessage(ws);
                }
                else
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                    Console.WriteLine("Falha na conexão WebSocket.");
                    return "Falha na conexão WebSocket.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return $"Exception: {ex.Message}";
            }
        }
        private async Task SendMessage(ClientWebSocket ws, Question question)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(question));
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        private async Task<string> ReceiveMessage(ClientWebSocket ws)
        {
            const int bufferSize = 1024;
            var buffer = new ArraySegment<byte>(new byte[bufferSize]);
            var messageBuilder = new StringBuilder();

            WebSocketReceiveResult result;
            do
            {
                result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var segment = new ArraySegment<byte>(buffer.Array, buffer.Offset, result.Count);
                    var messagePart = Encoding.UTF8.GetString(segment);
                    messageBuilder.Append(messagePart);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                }
            } while (!result.EndOfMessage);

            return messageBuilder.ToString();
        }

        public async Task DeleteSession(string session_id)
        {
            try
            {
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(new { session_id }),
                    Encoding.UTF8,
                    "application/json");

                using var response = await _httpClient.PostAsync("delete_session", jsonContent);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"{jsonResponse}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}