using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ADAPT.JohnDeere.api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ADAPT.JohnDeere.Controllers
{
    [ApiController]
    [Route("johndeere/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;

        // private readonly IModuleConfiguration moduleConfiguration;

        // public AuthenticationController(IConfiguration configuration, IModuleConfiguration moduleConfiguration)
        public AuthenticationController(IConfiguration configuration)
        {
            this.configuration = configuration;
            // this.moduleConfiguration = moduleConfiguration;
        }

        [HttpGet("cb")]
        public async Task<IActionResult> AuthCallbackMediation(string code, string state)
        {
            var authconfig = configuration.GetSection("johndeere:auth");
            var appid = authconfig.GetValue<string>("appId");
            var appsecret = authconfig.GetValue<string>("appSecret");

            var tokenUrl = authconfig.GetValue<string>("accessTokenUrl");
            var redirectUrl = authconfig.GetValue<string>("cbUrl");


            var client = new HttpClient();

            var tokenRequestParameters = $"grant_type=authorization_code&code={code}&redirect_uri={redirectUrl}";
            var requestData = new StringContent(tokenRequestParameters, Encoding.UTF8, "application/x-www-form-urlencoded");
            var basicauthtoken = Encoding.ASCII.GetBytes($"{appid}:{appsecret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(basicauthtoken));


            var response = await client.PostAsync(tokenUrl, requestData);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var tokenResponseText = await response.Content.ReadAsStringAsync();

                var tokenObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenResponseText);
                var otherclient = new HttpClient();
                var jdapi = "https://sandboxapi.deere.com/platform/organizations";
                var bearerauthtoke = tokenObject["access_token"];
                otherclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerauthtoke);
                otherclient.DefaultRequestHeaders.Accept.Clear();
                otherclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.deere.axiom.v3+json"));
                var orgresponse = await otherclient.GetAsync(jdapi);
                var orgresponsetext = await orgresponse.Content.ReadAsStringAsync();
                var orgresponseobj = JObject.Parse(orgresponsetext);
                var connectionslink = orgresponseobj.SelectTokens("$..links[?(@rel=='connections')].uri").FirstOrDefault();
                if (connectionslink != null)
                    return Redirect(connectionslink.Value<string>());
                // var tokenObject = JsonConvert.DeserializeObject<KeyrockTokenResponse>(tokenResponseText);
                var urlsep = state.IndexOf('?') >= 0 ? "&" : "?";
                // RedirectResult redirectResult = Redirect($"{state}{urlsep}_keyrock_token_response={tokenResponseText}");
                // return redirectResult;
                return new ContentResult
                {
                    ContentType = "text/html",
                    Content = $"<script>window.location.href = \"{state}{urlsep}_token_response={tokenResponseText.Replace("\"", "\\\"")}\";</script>"
                };
            }

            return NotFound();

        }
    }
}
