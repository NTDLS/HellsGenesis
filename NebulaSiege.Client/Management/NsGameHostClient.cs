using NebulaSiege.Client.Payloads;
using NebulaSiege.Client.Payloads.Response;
using Newtonsoft.Json;
using System.Text;

namespace NebulaSiege.Client.Management
{
    public class NsGameHostClient
    {
        private readonly NsClient _client;

        public NsGameHostClient(NsClient client)
        {
            _client = client;
        }

        public NsActionResponse Create(NsGameHost configuration)
        {
            string url = $"api/GameHost/{_client.SessionId}/Create";

            var postContent = new StringContent(JsonConvert.SerializeObject(configuration), Encoding.UTF8);

            using var response = _client.Connection.PostAsync(url, postContent);
            string resultText = response.Result.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<NsActionResponse>(resultText)
                ?? new NsActionResponseException("Invalid response.");
        }

        public NsActionResponseHostList GetList(NsGameHostFilter filter)
        {
            string url = $"api/GameHost/{_client.SessionId}/GetList";

            var postContent = new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8);

            using var response = _client.Connection.PostAsync(url, postContent);
            string resultText = response.Result.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<NsActionResponseHostList>(resultText)
                ?? new NsActionResponseHostList(new Exception("Invalid response."));
        }
    }
}
