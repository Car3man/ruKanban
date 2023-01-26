namespace RuKanban.Services.Api.Request.Ticket
{
    public class CreateTicketReqBody
    {
        public string title;
        public string description;

        public CreateTicketReqBody(string title, string description)
        {
            this.title = title;
            this.description = description;
        }
    }
}