namespace RuKanban.Services.Api.Request.Board
{
    public class CreateBoardReqBody
    {
        public string name;
        public string description;

        public CreateBoardReqBody(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
    }
}