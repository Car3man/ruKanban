namespace RuKanban.Services.Api.Request.Ticket
{
    public class MoveTicketReqBody
    {
        public string column_id;
        public string stand_after_id;

        public MoveTicketReqBody(string columnID, string standAfterId)
        {
            column_id = columnID;
            stand_after_id = standAfterId;
        }
    }
}