using System;

namespace RuKanban.Services.Api.Responses
{
    [Serializable]
    public class SignUpResponse : BaseResponse
    {
        public string user_id;
        public string login;
        public string access_token;
        public string refresh_token;
    }
}