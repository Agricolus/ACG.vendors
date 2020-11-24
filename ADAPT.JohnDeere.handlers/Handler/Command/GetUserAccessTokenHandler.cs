using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.core.Dto;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class GetUserAccessTokenHandler : IRequestHandler<GetUserAccessToken, AccessTokenResponse>
    {
        private readonly IConfiguration configuration;

        public GetUserAccessTokenHandler(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<AccessTokenResponse> Handle(GetUserAccessToken request, CancellationToken cancellationToken)
        {
            var authconfig = configuration.GetSection("johndeere:auth");
            var appid = authconfig.GetValue<string>("appId");
            var appsecret = authconfig.GetValue<string>("appSecret");

            var tokenUrl = authconfig.GetValue<string>("accessTokenUrl");
            var redirectUrl = authconfig.GetValue<string>("cbUrl");
            var apiUrl = authconfig.GetValue<string>("apiUrl");

            var client = new HttpClient();

            var tokenRequestParameters = $"grant_type=authorization_code&code={request.Code}&redirect_uri={redirectUrl}";
            var requestData = new StringContent(tokenRequestParameters, Encoding.UTF8, "application/x-www-form-urlencoded");
            var basicauthtoken = Encoding.ASCII.GetBytes($"{appid}:{appsecret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(basicauthtoken));

            var response = await client.PostAsync(tokenUrl, requestData);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var tokenResponseText = await response.Content.ReadAsStringAsync();

                var tokenObject = JsonConvert.DeserializeObject<AccessTokenResponse>(tokenResponseText);
                return tokenObject;
            }

            return null;
        }
    }
}
