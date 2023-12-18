using Microsoft.AspNetCore.Mvc;
using NebulaSiege.Client.Payloads;

namespace NebulaSiege.Server.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerController
    {
        [HttpGet]
        [Route("{sessionId}/Ping")]
        public NsActionResponsePing Ping(Guid sessionId)
        {
            try
            {
                var session = Program.Core.Sessions.Upsert(sessionId);

                var result = new NsActionResponsePing
                {
                    SessionId = sessionId,
                    ServerTimeUTC = DateTime.UtcNow,
                    Success = true
                };

                return result;
            }
            catch (Exception ex)
            {
                return new NsActionResponsePing
                {
                    ExceptionText = ex.Message,
                    Success = false
                };
            }
        }
    }
}
