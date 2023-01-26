using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api.Request.Workspace;

namespace RuKanban.Services.Api.Routes
{
    public class WorkspaceRoute : BaseRoute
    {
        public WorkspaceRoute(string basePath) : base(basePath)
        {
        }

        public ApiRequest GetWorkspaces()
        {
            var uri = GetUri("/workspace.get");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            return request;
        }

        public ApiRequest GetWorkspaceById(string workspaceId)
        {
            var uri = GetUri($"/workspace.getById?workspace_id={workspaceId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            return request;
        }

        public ApiRequest CreateWorkspace(string name)
        {
            var uri = GetUri("/workspace.create");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new CreateWorkspaceReqBody(name);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }

        public ApiRequest UpdateWorkspace(string workspaceId, string name, string[] usersToAdd, string[] usersToDelete)
        {
            var uri = GetUri($"/workspace.update?workspace_id={workspaceId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new UpdateWorkspaceReqBody(name, usersToAdd, usersToDelete);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }

        public ApiRequest GetWorkspaceUsers(string workspaceId)
        {
            var uri = GetUri($"/workspace.getUsers?workspace_id={workspaceId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            return request;
        }
    }
}