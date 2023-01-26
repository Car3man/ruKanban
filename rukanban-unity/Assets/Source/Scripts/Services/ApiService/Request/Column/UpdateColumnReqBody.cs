namespace RuKanban.Services.Api.Request.Column
{
    public class UpdateColumnReqBody
    {
        public int index;
        public string name;

        public UpdateColumnReqBody(int index, string name)
        {
            this.index = index;
            this.name = name;
        }
    }
}