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


        public async Task DeleteSession(Session session)
        {
            try
            {
                var webSocketClient = new ConnectClientService();
                await webSocketClient.DeleteSession(session.Question[0].session_id);
                Console.WriteLine($"Sessão {session.IdSession} excluída!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}