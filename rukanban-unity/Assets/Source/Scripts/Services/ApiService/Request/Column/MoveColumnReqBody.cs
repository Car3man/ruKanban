namespace RuKanban.Services.Api.Request.Column
{
    public class MoveColumnReqBody
    {
        public string stand_after_id;

        public MoveColumnReqBody(string standAfterId)
        {
            stand_after_id = standAfterId;
        }
    }
}