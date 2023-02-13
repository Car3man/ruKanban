namespace RuKanban.Services.Api.Request.Auth
{
    public class SignUpReqBody
    {
        public string login;
        public string password;
        public string first_name;
        public string sur_name; // TODO: rename to last name

        public SignUpReqBody(string login, string password, string firstName, string lastName)
        {
            this.login = login;
            this.password = password;
            first_name = firstName;
            sur_name = lastName;
        }
    }
}