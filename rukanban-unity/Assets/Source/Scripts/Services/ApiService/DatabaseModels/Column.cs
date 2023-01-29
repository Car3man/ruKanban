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
        
        public static bool IsTitleValid(string title)
        {
            string trimmedTitle = title.Trim();

            if (trimmedTitle.Length < 1)
            {
                return false;
            }

            if (trimmedTitle.Length > 36)
            {
                return false;
            }

            return true;
        }
    }
}