using Microsoft.AspNetCore.Mvc;
using NebulaSiege.Client.Payloads;
using NebulaSiege.Client.Payloads.Response;
using NebulaSiege.Shared;
using Newtonsoft.Json;

namespace NebulaSiege.Server.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameHostController
    {
        [HttpPost]
        [Route("{sessionId}/Create")]
        public NsActionResponse Create(Guid sessionId, [FromBody] string value)
        {
            try
            {
                var session = Program.Core.Sessions.Upsert(sessionId);
                var gameHost = JsonConvert.DeserializeObject<NsGameHost>(value);
                NsUtility.EnsureNotNull(gameHost);
                Program.Core.GameHost.Create(sessionId, gameHost);

                return new NsActionResponseSuccess();
            }
            catch (Exception ex)
            {
                return new NsActionResponseException(ex);
            }
        }

        [HttpPost]
        [Route("{sessionId}/GetList")]
        public NsActionResponseHostList GetList(Guid sessionId, [FromBody] string value)
        {
            try
            {
                var session = Program.Core.Sessions.Upsert(sessionId);
                var gameHostFilter = JsonConvert.DeserializeObject<NsGameHostFilter>(value);
                NsUtility.EnsureNotNull(gameHostFilter);

                return new NsActionResponseHostList()
                {
                    Collection = Program.Core.GameHost.GetList(sessionId, gameHostFilter)
                };
            }
            catch (Exception ex)
            {
                return new NsActionResponseHostList(ex);
            }
        }
    }
}
