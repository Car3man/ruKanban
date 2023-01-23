using System;

namespace RuKanban.Services.Api.RequestJsonBody
{
    [Serializable]
    public class SignUpRequest
    {
        public string login;
        public string password;
        public string first_name;
        public string sur_name;
        public string patronymic;

        public SignUpRequest(string login, string password, string firstName, string surName, string patronymic)
        {
            this.login = login;
            this.password = password;
            first_name = firstName;
            sur_name = surName;
            this.patronymic = patronymic;
        }
    }
}