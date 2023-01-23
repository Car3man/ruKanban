namespace RuKanban.Services.Authorization
{
    public struct AuthorizationData
    {
        public string AccessToken;
        public string RefreshToken;
        public string UserId;
        public string Login;

        public AuthorizationData(string accessToken, string refreshToken, string userId, string login)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            UserId = userId;
            Login = login;
        }

        public static AuthorizationData Empty()
        {
            return new AuthorizationData(string.Empty, string.Empty, string.Empty, string.Empty);
        }
    }
}