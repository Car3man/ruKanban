using System;

namespace RuKanban.Services.Api.RequestJsonBody
{
    [Serializable]
    public class ChangePasswordRequest
    {
        public string current_password;
        public string new_password;

        public ChangePasswordRequest(string currentPassword, string newPassword)
        {
            current_password = currentPassword;
            new_password = newPassword;
        }
    }
}