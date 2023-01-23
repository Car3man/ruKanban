using System;

namespace RuKanban.Services.Api.Responses
{
    [Serializable]
    public class CreateTicketResponse : BaseResponse
    {
        public string id;
        public string column_id;
        public int index;
        public string title;
        public string description;
        public string created_at;
    }
}