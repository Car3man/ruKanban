using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api.Request.Ticket;

namespace RuKanban.Services.Api.Routes
{
    public class TicketRoute : BaseRoute
    {
        public TicketRoute(string basePath) : base(basePath)
        {
        }

        public ApiRequest GetTickets(string columnId)
        {
            var uri = GetUri($"/ticket.get?column_id={columnId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            return request;
        }

        public ApiRequest GetTicketById(string ticketId)
        {
            var uri = GetUri($"/ticket.getById?ticket_id={ticketId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            return request;
        }

        public ApiRequest CreateTicket(string columnId, string title, string description)
        {
            var uri = GetUri($"/ticket.create?column_id={columnId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new CreateTicketReqBody(title, description);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
        
        public ApiRequest ChangeTitle(string ticketId, string title)
        {
            var uri = GetUri($"/ticket.changeTitle?ticket_id={ticketId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new ChangeTicketTitleReqBody(title);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
        
        public ApiRequest ChangeDescription(string ticketId, string description)
        {
            var uri = GetUri($"/ticket.changeDescription?ticket_id={ticketId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new ChangeTicketDescriptionReqBody(description);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }

        public ApiRequest MoveTicket(string ticketId, string columnId, string standAfterId)
        {
            var uri = GetUri($"/ticket.move?ticket_id={ticketId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new MoveTicketReqBody(columnId, standAfterId);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
        
        public ApiRequest DeleteTicket(string ticketId)
        {
            var uri = GetUri($"/ticket.delete?ticket_id={ticketId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            return request;
        }
    }
}