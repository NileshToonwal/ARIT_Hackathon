using System;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace ARIT_Hackathon.Extensions
{
    public class AuthorizeActionFilter : IAuthorizationFilter
    {
        private readonly string realm;
        public AuthorizeActionFilter(string realm = null)
        {
            this.realm = realm;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //throw new NotImplementedException();
            // Bearer 216d44d1b42d4461840ad6f505e7a167
            string TokenId = "";
            string authHeader = context.HttpContext.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Bearer"))
            {
                TokenId = authHeader.Substring("Bearer ".Length).Trim();
            }
            bool isAuthorized = GetTokenVerify(TokenId);
            if (isAuthorized == true)
            {

            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }

        public bool GetTokenVerify(string token)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("token", token);
            HttpResponseMessage response = httpClient.GetAsync($"{"http://192.168.52.112:9080/api/v1/token/CheckSessionToken"}").Result;
            httpClient.Dispose();
            string Result = response.Content.ReadAsStringAsync().Result;
            //return Result;
            if (Result != null && response.IsSuccessStatusCode != false)
            {
                SessionParams ObjResult = JsonConvert.DeserializeObject<SessionParams>(Result);
                return ObjResult.status;
            }
            else
            {
                return false;
            }
        }

        public class SessionParams
        {
            public string message { get; set; }
            public bool status { get; set; }
        }
    }
}
