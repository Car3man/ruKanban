using System;

namespace RuKanban.Services.Api.Responses
{
    [Serializable]
    public class CreateWorkspaceResponse : BaseResponse
    {
        public string id;
        public string name;
        public string created_at;
    }
}