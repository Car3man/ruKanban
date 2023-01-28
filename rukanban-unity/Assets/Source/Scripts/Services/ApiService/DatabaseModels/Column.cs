using System;

namespace RuKanban.Services.Api.DatabaseModels
{
    [Serializable]
    public class Column
    {
        public string id;
        public string board_id;
        public int index;
        public string title;
        public string created_at;
    }
}