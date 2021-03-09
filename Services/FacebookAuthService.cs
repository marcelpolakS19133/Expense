using Expense.Services;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;

namespace Expense
{
    public class FacebookAuthService : IAuthService
    {
        private readonly HttpClient httpClient;
        private readonly FBLoginConfig fbLoginConfig;
        private readonly string redirectUrl = "http://localhost:32009/api/Auth/step1";

        public FacebookAuthService(HttpClient httpClient, IOptions<FBLoginConfig> fbLoginConfigAccessor)
        {
            this.httpClient = httpClient;
            this.fbLoginConfig = fbLoginConfigAccessor.Value;
        }

        public object DoAuth(string code)
        {
            var url = "https://graph.facebook.com/v10.0/oauth/access_token?"
                        + $"client_id={fbLoginConfig.AppID}"
                        + $"&redirect_uri={redirectUrl}"
                        + $"&client_secret={fbLoginConfig.AppSecret}"
                        + $"&code={code}";
            var response = httpClient.Send(new HttpRequestMessage(HttpMethod.Get, url));

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}