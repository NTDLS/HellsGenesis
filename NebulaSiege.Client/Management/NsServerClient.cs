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
            if (result == null || result.Success == false)
            {
                throw new NsAPIResponseException(result == null ? "Invalid response" : result.ExceptionText);
            }

            return result;
        }

        public NsActionResponse CloseSession()
        {
            string url = $"api/Server/{_client.SessionId}/CloseSession";

            using var response = _client.Connection.GetAsync(url);
            string resultText = response.Result.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<NsActionResponse>(resultText);
            if (result == null || result.Success == false)
            {
                throw new NsAPIResponseException(result == null ? "Invalid response" : result.ExceptionText);
            }

            return result;
        }

    }
}
