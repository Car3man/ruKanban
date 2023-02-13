using System;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api.Request.Auth;

namespace RuKanban.Services.Api.Routes
{
    public class AuthRoute : BaseRoute
    {
        public AuthRoute(string basePath) : base(basePath)
        {
        }
        
        public ApiRequest SignUp(string login,
            string password, string firstName, string surName)
        {
            var uri = GetUri("/auth.signUp");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new SignUpReqBody(login, password, firstName, surName);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }

        public ApiRequest SignIn(string login, string password)
        {
            var uri = GetUri("/auth.signIn");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new SignInReqBody(login, password);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
    
        public ApiRequest SignOut()
        {
            var uri = GetUri("/auth.signOut");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            return request;
        }

        public ApiRequest ChangePassword(string currentPassword, string newPassword)
        {
            var uri = GetUri("/auth.changePassword");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new ChangePasswordReqBody(currentPassword, newPassword);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }

        public ApiRequest RefreshTokens(string refreshToken)
        {
            var uri = GetUri("/auth.refreshTokens");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new RefreshTokensReqBody(refreshToken);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
    }
}