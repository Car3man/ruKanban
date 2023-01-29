using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api.Request.Column;

namespace RuKanban.Services.Api.Routes
{
    public class ColumnRoute : BaseRoute
    {
        public ColumnRoute(string basePath) : base(basePath)
        {
        }
        
        public ApiRequest GetColumns(string boardId)
        {
            var uri = GetUri($"/column.get?board_id={boardId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            return request;
        }
        
        public ApiRequest CreateColumn(string boardId, string title)
        {
            var uri = GetUri($"/column.create?board_id={boardId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new CreateColumnReqBody(title);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }

        public ApiRequest ChangeTitle(string columnId, string title)
        {
            var uri = GetUri($"/column.changeTitle?column_id={columnId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new ChangeColumnTitleReqBody(title);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
        
        public ApiRequest MoveColumn(string columnId, string standAfterId)
        {
            var uri = GetUri($"/column.move?column_id={columnId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            var requestBody = new MoveColumnReqBody(standAfterId);
            request.SetJsonBody(JsonConvert.SerializeObject(requestBody));
            return request;
        }
        
        public ApiRequest DeleteColumn(string columnId)
        {
            var uri = GetUri($"/column.delete?column_id={columnId}");
            var request = new ApiRequest(uri, HTTPMethods.Post);
            return request;
        }
    }
}