using Newtonsoft.Json;
using NebulaSiege.Shared.Exceptions;
using NebulaSiege.Client.Payloads.Response;

namespace NebulaSiege.Client.Management
{
    public class NsServerClient
    {
        private readonly NsClient _client;

        public NsServerClient(NsClient client)
        {
            _client = client;
        }

        public NsActionResponsePing Ping()
        {
            string url = $"api/Server/{_client.SessionId}/Ping";

            using var response = _client.Connection.GetAsync(url);
            string resultText = response.Result.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<NsActionResponsePing>(resultText);

            return JsonConvert.DeserializeObject<NsActionResponsePing>(resultText)
                ?? new NsActionResponsePing(new Exception("Invalid response."));
        }

        public NsActionResponse CloseSession()
        {
            string url = $"api/Server/{_client.SessionId}/CloseSession";

            using var response = _client.Connection.GetAsync(url);
            string resultText = response.Result.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<NsActionResponse>(resultText)
                ?? new NsActionResponseException("Invalid response.");
        }

    }
}
