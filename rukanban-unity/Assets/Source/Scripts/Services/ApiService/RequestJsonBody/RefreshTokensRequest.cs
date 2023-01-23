using System;

namespace RuKanban.Services.Api.RequestJsonBody
{
    [Serializable]
    public class RefreshTokensRequest
    {
        public string refresh_token;

        public RefreshTokensRequest(string refreshToken)
        {
            refresh_token = refreshToken;
        }
    }
}