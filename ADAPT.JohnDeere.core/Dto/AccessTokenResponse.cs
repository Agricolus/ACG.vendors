
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace ADAPT.JohnDeere.core.Dto
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class AccessTokenResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
    }
}
