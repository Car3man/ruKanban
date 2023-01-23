using System;

namespace RuKanban.Services.Api.RequestJsonBody
{
    [Serializable]
    public class SignInRequest
    {
        public string login;
        public string password;

        public SignInRequest(string login, string password)
        {
            this.login = login;
            this.password = password;
        }
    }
}