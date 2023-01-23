using System;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api.RequestJsonBody;
using RuKanban.Services.AppConfiguration;
using Zenject;

namespace RuKanban.Services.Api
{
    public class ApiService
    {
        private readonly AppConfigurationService _appConfigurationService;

        [Inject]
        public ApiService(AppConfigurationService appConfigurationService)
        {
            _appConfigurationService = appConfigurationService;
        }

        private string GetBaseApiUrl()
        {
            AppConfigurationService.ApiScheme scheme = _appConfigurationService.Api;
            return $"{scheme.Host}:{scheme.Port}{scheme.BaseUrl}";
        }
    
        public ApiRequest SignUp(string login,
            string password, string firstName, string surName, string patronymic)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/auth.signUp"), HTTPMethods.Post);
            var requestBody = new SignUpRequest(login, password, firstName, surName, patronymic);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }

        public ApiRequest SignIn(string login, string password)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/auth.signIn"), HTTPMethods.Post);
            var requestBody = new SignInRequest(login, password);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
    
        public ApiRequest SignOut()
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/auth.signOut"), HTTPMethods.Post);
            return request;
        }

        public ApiRequest ChangePassword(string currentPassword, string newPassword)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/auth.changePassword"), HTTPMethods.Post);
            var requestBody = new ChangePasswordRequest(currentPassword, newPassword);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }

        public ApiRequest RefreshTokens(string refreshToken)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/auth.refreshTokens"), HTTPMethods.Post);
            var requestBody = new RefreshTokensRequest(refreshToken);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
        
        public ApiRequest GetWorkspaces()
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/workspace.get"), HTTPMethods.Post);
            return request;
        }
        
        public ApiRequest GetWorkspace(string workspaceId)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/workspace.getById?workspace_id={workspaceId}"), HTTPMethods.Post);
            return request;
        }
        
        public ApiRequest GetWorkspaceUsers(string workspaceId)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/workspace.getUsers?workspace_id={workspaceId}"), HTTPMethods.Post);
            return request;
        }
        
        public ApiRequest CreateWorkspace(string name)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/workspace.create"), HTTPMethods.Post);
            var requestBody = new CreateWorkspaceRequest(name);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
        
        public ApiRequest UpdateWorkspace(string workspaceId, string name, string[] usersToAdd, string[] usersToDelete)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/workspace.update?workspace_id={workspaceId}"), HTTPMethods.Post);
            var requestBody = new UpdateWorkspaceRequest(name, usersToAdd, usersToDelete);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
        
        public ApiRequest GetBoards(string workspaceId)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/board.get?workspace_id={workspaceId}"), HTTPMethods.Post);
            return request;
        }
        
        public ApiRequest CreateBoard(string workspaceId, string name, string description)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/board.create?workspace_id={workspaceId}"), HTTPMethods.Post);
            var requestBody = new CreateBoardRequest(name, description);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
        
        public ApiRequest GetColumns(string boardId)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/column.get?board_id={boardId}"), HTTPMethods.Post);
            return request;
        }
        
        public ApiRequest CreateColumn(string boardId, string name)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/column.create?board_id={boardId}"), HTTPMethods.Post);
            var requestBody = new CreateColumnRequest(name);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
        
        public ApiRequest DeleteColumn(string columnId)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/column.delete?column_id={columnId}"), HTTPMethods.Post);
            return request;
        }
        
        public ApiRequest GetTicket(string ticketId)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/ticket.getById?ticket_id={ticketId}"), HTTPMethods.Post);
            return request;
        }
        
        public ApiRequest GetTickets(string columnId)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/ticket.get?column_id={columnId}"), HTTPMethods.Post);
            return request;
        }
        
        public ApiRequest CreateTicket(string columnId, string title, string description)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/ticket.create?column_id={columnId}"), HTTPMethods.Post);
            var requestBody = new CreateTicketRequest(title, description);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
        
        public ApiRequest UpdateTicket(string ticketId,
            string columnId, int? index,
            string title, string description)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/ticket.update?ticket_id={ticketId}"), HTTPMethods.Post);
            var requestBody = new UpdateTicketRequest(columnId, index, title, description);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
        
        public ApiRequest DeleteTicket(string ticketId)
        {
            var request = new ApiRequest(new Uri($"{GetBaseApiUrl()}/ticket.delete?ticket_id={ticketId}"), HTTPMethods.Post);
            return request;
        }
    }
}