using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api.Request.Board;

namespace RuKanban.Services.Api.Routes
{
    public class BoardRoute : BaseRoute
    {
        public BoardRoute(string basePath) : base(basePath)
        {
        }
        
        public ApiRequest GetBoards(string workspaceId)
        {
            var uri = GetUri($"/board.get?workspace_id={workspaceId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            return request;
        }
        
        public ApiRequest CreateBoard(string workspaceId, string name, string description)
        {
            var uri = GetUri($"/board.create?workspace_id={workspaceId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new CreateBoardReqBody(name, description);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
    }
}