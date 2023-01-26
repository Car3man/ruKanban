namespace RuKanban.Services.Api.Request.Column
{
    public class CreateColumnReqBody
    {
        public string name;

        public CreateColumnReqBody(string name)
        {
            this.name = name;
        }
    }
}