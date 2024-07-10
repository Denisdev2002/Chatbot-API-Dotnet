using Domain.Service.Services.ServiceApiExternal;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Domain.Entities;
using Microsoft.VisualBasic;

namespace Infra.Repositories
{
    public class RequestSessionService
    {
        private readonly ConnectClientService _connectClientService;

        public RequestSessionService()
        {
            _connectClientService = new ConnectClientService();
        }

        public async Task<List<Conversation>> GetSessionId(string session_id)
        {
            try
            {
                var conversations = await _connectClientService.GetSessionId(session_id);
                return conversations;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return new List<Conversation>();
            }
        }
    }
}