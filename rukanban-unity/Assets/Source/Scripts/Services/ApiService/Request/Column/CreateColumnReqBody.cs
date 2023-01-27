namespace RuKanban.Services.Api.Request.Column
{
    public class CreateColumnReqBody
    {
        public string title;

        public CreateColumnReqBody(string title)
        {
            this.title = title;
        }
    }
}