using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.core.CQRS.Query;
using ADAPT.JohnDeere.core.Dto;
using ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse;
using ADAPT.JohnDeere.core.Service;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ADAPT.JohnDeere.Controllers
{
    [ApiController]
    [Route("johndeere/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IMediator mediator;
        private readonly IJDApiClient apiclient;

        // private readonly IModuleConfiguration moduleConfiguration;

        // public AuthenticationController(IConfiguration configuration, IModuleConfiguration moduleConfiguration)
        public AuthenticationController(IConfiguration configuration, IMediator mediator, IJDApiClient apiclient)
        {
            this.configuration = configuration;
            // this.moduleConfiguration = moduleConfiguration;
            this.mediator = mediator;
            this.apiclient = apiclient;
        }

        [HttpGet("cb")]
        public async Task<IActionResult> AuthCallbackMediation(string code, string state)
        {
            var accessToken = await mediator.Send(new GetUserAccessToken() { Code = code });

            if (accessToken != null)
            {
                var orgresponseobj = await apiclient.Get<Response<Organization>>("/organizations", accessToken.AccessToken);
                if (orgresponseobj == null)
                    return BadRequest("unable to retrieve user organizations");

                var connectionslink = orgresponseobj.Values.SelectMany(o => o.Links.Where(l => l.Rel == "connections").Select(l => l.Uri)).FirstOrDefault();
                if (connectionslink != null)
                    return Redirect(connectionslink);

                var urlsep = state.IndexOf('?') >= 0 ? "&" : "?";
                var tokenResponseText = JsonConvert.SerializeObject(accessToken);
                return new ContentResult
                {
                    ContentType = "text/html",
                    Content = $"<script>window.location.href = \"{state}{urlsep}_token_response={tokenResponseText.Replace("\"", "\\\"")}\";</script>"
                };
            }

            return NotFound();

        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserToken userdata)
        {
            // var authconfig = configuration.GetSection("johndeere:auth");
            // var apiUrl = authconfig.GetValue<string>("apiUrl");

            // var client = new HttpClient();

            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userdata.AccessToken);
            // client.DefaultRequestHeaders.Accept.Clear();
            // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.deere.axiom.v3+json"));

            // var usersresponse = await client.GetAsync($"{apiUrl}/users/@currentUser");

            var usersresponse = await this.apiclient.Get<User>("/users/@currentUser", userdata.AccessToken);

            if (usersresponse == null)
            {
                return BadRequest();
            }
            // var responseData = JObject.Parse(await usersresponse.Content.ReadAsStringAsync());
            var accountName = usersresponse.AccountName;
            await mediator.Send(new CreateOrUpdateUserRegistration()
            {
                ExternalUserId = accountName,
                UserId = userdata.UserId,
                AccessToken = userdata.AccessToken,
                RefreshToken = userdata.RefreshToken,
                ExpiresIn = userdata.ExpiresIn
            });

            return Ok();

        }

        [HttpGet("token/{userId}")]
        public async Task<IActionResult> CheckUserToken(string userId)
        {
            var userToken = await mediator.Send(new GetUserToken() { UserId = userId });
            if (userToken == null)
                return Ok(false);
                
            var usersresponse = await this.apiclient.Get<User>("/users/@currentUser", userToken.AccessToken);
            if (usersresponse == null)
            {
                userToken = await mediator.Send(new RefreshUserAccessToken() { RefreshToken = userToken.RefreshToken });
                if (userToken == null)
                    return Ok(false);
            }
            await mediator.Send(new CreateOrUpdateUserRegistration()
            {
                UserId = userId,
                AccessToken = userToken.AccessToken,
                RefreshToken = userToken.RefreshToken,
                ExpiresIn = userToken.ExpiresIn
            });
            return Ok(true);
        }

    }
}
