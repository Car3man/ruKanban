using System;

namespace RuKanban.Services.Api.RequestJsonBody
{
    [Serializable]
    public class CreateBoardRequest
    {
        public string name;
        public string description;

        public CreateBoardRequest(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }
}