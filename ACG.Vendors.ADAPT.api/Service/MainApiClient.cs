using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ACG.Common.Service;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ACG.Vendors.ADAPT.api.Service
{
    public class MainApiClient : IMainApiClient
    {
        private readonly IConfiguration configuration;

        public MainApiClient(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<T> Get<T>(string endpoint)
        {
            var apiUrl = configuration.GetValue<string>("mainApi");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var apiresponse = await client.GetAsync($"{apiUrl}/{endpoint}");
            if (apiresponse.StatusCode != HttpStatusCode.OK)
                return default(T);

            var apiresponsetext = await apiresponse.Content.ReadAsStringAsync();
            var apiresponseobj = JsonConvert.DeserializeObject<T>(apiresponsetext);

            return apiresponseobj;
        }

        public async Task<T> Post<T>(string endpoint, Object payload)
        {
            var apiUrl = configuration.GetValue<string>("mainApi");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var objAsJson = JsonConvert.SerializeObject(payload);
            //var objAsJson = JsonConvert.SerializeObject(myObject);
            var content = new StringContent(objAsJson, Encoding.UTF8, "application/json");

            var apiresponse = await client.PostAsync($"{apiUrl}/{endpoint}", content);
            if (apiresponse.StatusCode != HttpStatusCode.OK)
                throw new MainApiException($"main api error: {apiresponse.StatusCode}", new Exception(await apiresponse.Content.ReadAsStringAsync()));
                // return default(T);

            var apiresponsetext = await apiresponse.Content.ReadAsStringAsync();
            var apiresponseobj = JsonConvert.DeserializeObject<T>(apiresponsetext);

            return apiresponseobj;
        }
    }

    class MainApiException : Exception
    {
        public MainApiException()
        {
        }

        public MainApiException(string message)
            : base(message)
        {
        }

        public MainApiException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
