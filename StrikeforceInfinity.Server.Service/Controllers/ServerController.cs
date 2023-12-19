using Microsoft.AspNetCore.Mvc;
using StrikeforceInfinity.Client.Payloads.Response;

namespace StrikeforceInfinity.Server.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerController
    {
        [HttpGet]
        [Route("{sessionId}/Ping")]
        public SiActionResponsePing Ping(Guid sessionId)
        {
            try
            {
                var session = Program.Core.Sessions.Upsert(sessionId);

                var result = new SiActionResponsePing
                {
                    SessionId = sessionId,
                    ServerTimeUTC = DateTime.UtcNow,
                    Success = true
                };

                return result;
            }
            catch (Exception ex)
            {
                return new SiActionResponsePing
                {
                    ExceptionText = ex.Message,
                    Success = false
                };
            }
        }
    }
}
