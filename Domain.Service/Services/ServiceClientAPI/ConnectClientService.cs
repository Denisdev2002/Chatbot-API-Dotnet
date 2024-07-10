using Domain.Entities;
using Domain.ViewModel;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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

                if (ws.State == WebSocketState.Open)
                {
                    if (string.IsNullOrEmpty(question.session_id) || !Guid.TryParse(question.session_id, out _))
                    {
                        question.session_id = Guid.NewGuid().ToString();
                    }

                    await SendMessage(ws, question);
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
        public async Task<List<Conversation>> GetSessionId(string session_id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"sessions/{session_id}");
                Console.WriteLine($"Sessão id: {session_id}");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var conversations = JsonSerializer.Deserialize<List<Conversation>>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return conversations ?? new List<Conversation>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }
    }
}