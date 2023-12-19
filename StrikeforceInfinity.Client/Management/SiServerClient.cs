using Newtonsoft.Json;
using StrikeforceInfinity.Client.Payloads.Response;

namespace StrikeforceInfinity.Client.Management
{
    public class SiServerClient
    {
        private readonly SiClient _client;

        public SiServerClient(SiClient client)
        {
            _client = client;
        }

        public SiActionResponsePing Ping()
        {
            string url = $"api/Server/{_client.SessionId}/Ping";

            using var response = _client.Connection.GetAsync(url);
            string resultText = response.Result.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<SiActionResponsePing>(resultText);

            return JsonConvert.DeserializeObject<SiActionResponsePing>(resultText)
                ?? new SiActionResponsePing(new Exception("Invalid response."));
        }

        public SiActionResponse CloseSession()
        {
            string url = $"api/Server/{_client.SessionId}/CloseSession";

            using var response = _client.Connection.GetAsync(url);
            string resultText = response.Result.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<SiActionResponse>(resultText)
                ?? new SiActionResponseException("Invalid response.");
        }

    }
}
