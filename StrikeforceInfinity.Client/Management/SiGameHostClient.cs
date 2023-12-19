using Newtonsoft.Json;
using StrikeforceInfinity.Client.Payloads;
using StrikeforceInfinity.Client.Payloads.Response;
using System.Text;

namespace StrikeforceInfinity.Client.Management
{
    public class SiGameHostClient
    {
        private readonly SiClient _client;

        public SiGameHostClient(SiClient client)
        {
            _client = client;
        }

        public SiActionResponse Create(SiGameHost configuration)
        {
            string url = $"api/GameHost/{_client.SessionId}/Create";

            var postContent = new StringContent(JsonConvert.SerializeObject(configuration), Encoding.UTF8);

            using var response = _client.Connection.PostAsync(url, postContent);
            string resultText = response.Result.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<SiActionResponse>(resultText)
                ?? new SiActionResponseException("Invalid response.");
        }

        public SiActionResponseHostList GetList(SiGameHostFilter filter)
        {
            string url = $"api/GameHost/{_client.SessionId}/GetList";

            var postContent = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8);

            using var response = _client.Connection.PostAsync(url, postContent);
            string resultText = response.Result.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<SiActionResponseHostList>(resultText)
                ?? new SiActionResponseHostList(new Exception("Invalid response."));
        }
    }
}
