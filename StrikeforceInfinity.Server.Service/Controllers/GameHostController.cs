using Microsoft.AspNetCore.Mvc;
using StrikeforceInfinity.Client.Payloads;
using StrikeforceInfinity.Client.Payloads.Response;
using StrikeforceInfinity.Shared;
using Newtonsoft.Json;

namespace StrikeforceInfinity.Server.Service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameHostController
    {
        [HttpPost]
        [Route("{sessionId}/Create")]
        public SiActionResponse Create(Guid sessionId, [FromBody] string value)
        {
            try
            {
                var session = Program.Core.Sessions.Upsert(sessionId);
                var gameHost = JsonConvert.DeserializeObject<SiGameHost>(value);
                SiUtility.EnsureNotNull(gameHost);
                Program.Core.GameHost.Create(sessionId, gameHost);

                return new SiActionResponseSuccess();
            }
            catch (Exception ex)
            {
                return new SiActionResponseException(ex);
            }
        }

        [HttpPost]
        [Route("{sessionId}/GetList")]
        public SiActionResponseHostList GetList(Guid sessionId, [FromBody] string value)
        {
            try
            {
                var session = Program.Core.Sessions.Upsert(sessionId);
                var gameHostFilter = JsonConvert.DeserializeObject<SiGameHostFilter>(value);
                SiUtility.EnsureNotNull(gameHostFilter);

                return new SiActionResponseHostList()
                {
                    Collection = Program.Core.GameHost.GetList(sessionId, gameHostFilter)
                };
            }
            catch (Exception ex)
            {
                return new SiActionResponseHostList(ex);
            }
        }
    }
}
