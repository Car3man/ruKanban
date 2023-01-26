namespace RuKanban.Services.Api.Request.Auth
{
    public class SignInReqBody
    {
        public string login;
        public string password;

        public SignInReqBody(string login, string password)
        {
            this.login = login;
            this.password = password;
        }
    }
}