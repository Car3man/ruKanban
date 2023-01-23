using System;

namespace RuKanban.Services.Api.RequestJsonBody
{
    [Serializable]
    public class UpdateTicketRequest
    {
        public string column_id;
        public int? index;
        public string title;
        public string description;

        public UpdateTicketRequest(string columnId, int? index, string title, string description)
        {
            column_id = columnId;
            this.index = index;
            this.title = title;
            this.description = description;
        }
    }
}