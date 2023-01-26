namespace RuKanban.Services.Api.Request.Ticket
{
    public class MoveTicketReqBody
    {
        public string column_id;
        public int index;

        public MoveTicketReqBody(string columnID, int index)
        {
            column_id = columnID;
            this.index = index;
        }
    }
}