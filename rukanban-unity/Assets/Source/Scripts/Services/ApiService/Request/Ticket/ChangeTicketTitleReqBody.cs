namespace RuKanban.Services.Api.Request.Ticket
{
    public class ChangeTicketTitleReqBody
    {
        public string title;

        public ChangeTicketTitleReqBody(string title)
        {
            this.title = title;
        }
    }
}