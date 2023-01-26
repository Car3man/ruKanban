namespace RuKanban.Services.Api.Request.Ticket
{
    public class ChangeTicketDescriptionReqBody
    {
        public string description;

        public ChangeTicketDescriptionReqBody(string description)
        {
            this.description = description;
        }
    }
}