using System;

namespace RuKanban.Services.Api.Responses
{
    [Serializable]
    public class CreateColumnResponse : BaseResponse
    {
        public string id;
        public string board_id;
        public int index;
        public string name;
        public string created_at;
    }
}