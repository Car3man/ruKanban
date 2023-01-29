namespace RuKanban.Services.Api.Request.Column
{
    public class ChangeColumnTitleReqBody
    {
        public string title;

        public ChangeColumnTitleReqBody(string title)
        {
            this.title = title;
        }
    }
}