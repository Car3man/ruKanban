namespace RuKanban.Services.Api.Request.Auth
{
    public class ChangePasswordReqBody
    {
        public string current_password;
        public string new_password;

        public ChangePasswordReqBody(string currentPassword, string newPassword)
        {
            current_password = currentPassword;
            new_password = newPassword;
        }
    }
}