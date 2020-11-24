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
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class RefreshUserAccessTokenHandler : IRequestHandler<RefreshUserAccessToken, UserToken>
    {
        private readonly IConfiguration configuration;

        public RefreshUserAccessTokenHandler(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        public async Task<UserToken> Handle(RefreshUserAccessToken request, CancellationToken cancellationToken)
        {
            var authconfig = configuration.GetSection("johndeere:auth");
            var appid = authconfig.GetValue<string>("appId");
            var appsecret = authconfig.GetValue<string>("appSecret");

            var tokenUrl = authconfig.GetValue<string>("accessTokenUrl");
            var client = new HttpClient();

            var tokenRequestParameters = $"grant_type=refresh_token&refresh_token={request.RefreshToken}";
            var requestData = new StringContent(tokenRequestParameters, Encoding.UTF8, "application/x-www-form-urlencoded");
            var basicauthtoken = Encoding.ASCII.GetBytes($"{appid}:{appsecret}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(basicauthtoken));

            var response = await client.PostAsync(tokenUrl, requestData);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var tokenResponseText = await response.Content.ReadAsStringAsync();
                JObject tokenObject = JObject.Parse(tokenResponseText);
                return new UserToken()
                {
                    AccessToken = tokenObject.SelectToken("access_token").Value<string>(),
                    ExpiresIn = tokenObject.SelectToken("expires_in").Value<int>(),
                    RefreshToken = tokenObject.SelectToken("refresh_token").Value<string>()
                };
            }
            return null;
        }
    }
}
