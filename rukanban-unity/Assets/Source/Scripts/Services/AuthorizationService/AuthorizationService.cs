using RuKanban.Services.Api;
using UnityEngine;

namespace RuKanban.Services.Authorization
{
    public class AuthorizationService
    {
        private readonly ApiRequestInterceptor _authorizationInterceptor;
        private AuthorizationData _authorizationData;

        public AuthorizationData AuthorizationData
        {
            get => _authorizationData;
            set
            {
                _authorizationData = value;
                SaveAuthorizationData();
            }
        }
    
        public AuthorizationService()
        {
            LoadLastAuthorizationData();
        
            _authorizationInterceptor = new ApiRequestInterceptor((req) =>
            {
                req.AddHeader("Authorization", $"Bearer {AuthorizationData.AccessToken}");
                return req;
            });
        }

        public ApiRequest GetRequestWithAuthorization(ApiRequest request)
        {
            return _authorizationInterceptor.Intercept(request);
        }

        private void SaveAuthorizationData()
        {
            PlayerPrefs.SetString("access_token", AuthorizationData.AccessToken);
            PlayerPrefs.SetString("refresh_token", AuthorizationData.RefreshToken);
            PlayerPrefs.SetString("user_id", AuthorizationData.UserId);
            PlayerPrefs.SetString("login", AuthorizationData.Login);
            PlayerPrefs.Save();
        }

        private void LoadLastAuthorizationData()
        {
            string accessToken = PlayerPrefs.GetString("access_token");
            string refreshToken = PlayerPrefs.GetString("refresh_token");
            string userId = PlayerPrefs.GetString("user_id");
            string login = PlayerPrefs.GetString("login");

            AuthorizationData = new AuthorizationData(accessToken, refreshToken, userId, login);
        }
    }
}