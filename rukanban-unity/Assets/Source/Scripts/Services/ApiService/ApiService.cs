using RuKanban.Services.Api.Routes;
using RuKanban.Services.AppConfiguration;
using Zenject;

namespace RuKanban.Services.Api
{
    public class ApiService
    {
        public readonly AuthRoute Auth;
        public readonly WorkspaceRoute Workspace;
        public readonly BoardRoute Board;
        public readonly ColumnRoute Column;
        public readonly TicketRoute Ticket;

        [Inject]
        public ApiService(AppConfigurationService appConfigurationService)
        {
            AppConfigurationService.ApiScheme scheme = appConfigurationService.Api;
            string basePath = $"{scheme.Host}:{scheme.Port}{scheme.BaseUrl}";

            Auth = new AuthRoute(basePath);
            Workspace = new WorkspaceRoute(basePath);
            Board = new BoardRoute(basePath);
            Column = new ColumnRoute(basePath);
            Ticket = new TicketRoute(basePath);
        }
    }
}