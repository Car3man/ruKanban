namespace RuKanban.Services.Api.Response.Auth
{
    public class SignUpRes : BaseRes
    {
        public string user_id;
        public string login;
        public string access_token;
        public string refresh_token;
    }
}