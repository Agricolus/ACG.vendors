using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.Service;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ADAPT.JohnDeere.handlers.Service
{
    public class JDApiClient : IJDApiClient
    {
        private readonly IConfiguration configuration;

        public JDApiClient(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<T> Get<T>(string endpoint, string accessToken)
        {
            var authconfig = configuration.GetSection("johndeere:auth");
            var apiUrl = authconfig.GetValue<string>("apiUrl");
            var client = new HttpClient();

            var jdapi = $"{apiUrl}/{endpoint}";
            if (endpoint.StartsWith("http"))
                jdapi = endpoint;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.deere.axiom.v3+json"));
            var apiresponse = await client.GetAsync(jdapi);
            if (apiresponse.StatusCode != HttpStatusCode.OK)
                return default(T);

            var apiresponsetext = await apiresponse.Content.ReadAsStringAsync();
            var apiresponseobj = JsonConvert.DeserializeObject<T>(apiresponsetext);

            return apiresponseobj;
        }
    }
}
