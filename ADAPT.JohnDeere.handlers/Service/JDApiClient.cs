using System;
using System.IO;
using System.IO.Compression;
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
            var authConfig = configuration.GetSection("johndeere:auth");
            var apiUrl = authConfig.GetValue<string>("apiUrl");
            var client = new HttpClient();

            var jdapi = $"{apiUrl}/{endpoint}";
            if (endpoint.StartsWith("http"))
                jdapi = endpoint;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.deere.axiom.v3+json"));
            var apiResponse = await client.GetAsync(jdapi);
            if (apiResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception(await apiResponse.Content.ReadAsStringAsync());

            var apiResponseText = await apiResponse.Content.ReadAsStringAsync();
            var apiResponseObj = JsonConvert.DeserializeObject<T>(apiResponseText);

            return apiResponseObj;
        }

        public async Task Download(string endpoint, string fileName, string accessToken)
        {
            var authConfig = configuration.GetSection("johndeere:auth");
            var apiUrl = authConfig.GetValue<string>("apiUrl");
            var client = new HttpClient();

            var jdapi = $"{apiUrl}/{endpoint}";
            if (endpoint.StartsWith("http"))
                jdapi = endpoint;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/zip"));
            var apiResponse = await client.GetAsync(jdapi);
            if (apiResponse.StatusCode != HttpStatusCode.OK)
                throw new Exception(await apiResponse.Content.ReadAsStringAsync());
            var apiResponseStream = await apiResponse.Content.ReadAsStreamAsync();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            var fileStream = File.OpenWrite(fileName);
            await apiResponseStream.CopyToAsync(fileStream);
            fileStream.Close();
            return;
        }
    }
}
