using System;

namespace RuKanban.Services.Api.Responses
{
    [Serializable]
    public class RefreshTokensResponse : BaseResponse
    {
        public string access_token;
        public string refresh_token;
    }
}