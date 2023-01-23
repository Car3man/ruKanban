using System;

namespace RuKanban.Services.Api.RequestJsonBody
{
    [Serializable]
    public class CreateTicketRequest
    {
        public string title;
        public string description;

        public CreateTicketRequest(string title, string description)
        {
            this.title = title;
            this.description = description;
        }
    }
}