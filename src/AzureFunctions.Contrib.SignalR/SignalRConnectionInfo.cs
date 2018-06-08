using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AzureFunctions.Contrib.SignalR
{
    public class SignalRConnectionInfo
    {
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty("accessKey")]
        public string AccessKey { get; set; }
    }

    public class SignalRConnectionBuilder
    {
        /// <summary>
        /// Your Azure SignalR service. 
        /// Will be something like 'example.service.signalr.net'. In that case the value of this property should be 'example'
        /// </summary>
        //[AppSetting(Default = "SignalR")]
        public string ServiceName { get; set; }

        public int ConnectionValidity { get; set; } = 30;

        /// <summary>
        /// Generates a token for the Azure SignalR REST API
        /// </summary>
        /// <param name="url"></param>
        /// <param name="accessKey"></param>
        /// <returns></returns>
        private string GetToken(string url)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AccessKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.CreateJwtSecurityToken(
                issuer: null,
                audience: url,
                subject: null,
                expires: DateTime.UtcNow.AddMinutes(ConnectionValidity),
                signingCredentials: credentials);
            return jwtTokenHandler.WriteToken(token);
        }

        public SignalRConnectionInfo CreateConnectionInfo(string hubName)
        {
            var url = $"https://{ServiceName}.service.signalr.net:5001/client/?hub={hubName}";
            return new SignalRConnectionInfo
            {
                Endpoint = url,
                AccessKey = GetToken(url)
            };
        }

        /// <summary>
        /// SignalR hub name        
        /// In case of you client code like:
        ///     var connection = new signalR.HubConnectionBuilder()
        ///                      .withUrl('/chat')
        ///                      .build();
        /// The value will be 'chat'
        /// </summary>
        // [AppSetting(Default = "SignalRHub")]
        public string Hub { get; set; }

        /// <summary>
        /// SignalR Access Key
        /// Retrieve it from the portal, in the SignalR Azure service under properties
        /// </summary>
       // [AppSetting(Default = "SignalRAccessKey")]
        public string AccessKey { get; set; }
    }
}
