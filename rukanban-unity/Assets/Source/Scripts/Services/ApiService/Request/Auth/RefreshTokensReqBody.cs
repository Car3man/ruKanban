namespace RuKanban.Services.Api.Request.Auth
{
    public class RefreshTokensReqBody
    {
        public string refresh_token;

        public RefreshTokensReqBody(string refreshToken)
        {
            refresh_token = refreshToken;
        }
    }
}