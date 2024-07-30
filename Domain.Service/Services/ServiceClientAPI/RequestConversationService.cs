using Domain.Entities;

namespace Domain.Service.Services.ServiceApiExternal
{
    public class RequestConversationService
    {
        public async Task<string> MakePostRequest(Question question)
        {
            try
            {
                var webSocketClient = new ConnectClientService();
                return await webSocketClient.ConnectAndSend(question);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }


        public async Task DeleteSession(string session)
        {
            try
            {
                var webSocketClient = new ConnectClientService();
                await webSocketClient.DeleteSession(session);
                Console.WriteLine($"Sessão {session} excluída!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}