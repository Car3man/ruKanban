using System;

namespace RuKanban.Services.Api.Responses
{
    [Serializable]
    public class CreateBoardResponse : BaseResponse
    {
        public string id;
        public string workspace_id;
        public string name;
        public string description;
        public string created_at;
    }
}