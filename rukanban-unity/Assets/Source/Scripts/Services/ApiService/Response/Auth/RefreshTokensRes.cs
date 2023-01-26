namespace RuKanban.Services.Api.Response.Auth
{
    public class RefreshTokensRes : BaseRes
    {
        public string access_token;
        public string refresh_token;
    }
}