using System;

namespace RuKanban.Services.Api.DatabaseModels
{
    [Serializable]
    public class Ticket
    {
        public string id;
        public string column_id;
        public int index;
        public string title;
        public string description;
        public string created_at;
    }
}