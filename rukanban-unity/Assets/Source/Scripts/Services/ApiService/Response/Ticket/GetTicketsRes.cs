namespace RuKanban.Services.Api.Response.Ticket
{
    public class GetTicketsRes : BaseRes
    {
        public DatabaseModels.Ticket[] tickets;
    }
}