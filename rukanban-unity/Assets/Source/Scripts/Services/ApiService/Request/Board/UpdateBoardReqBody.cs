namespace RuKanban.Services.Api.Request.Board
{
    public class UpdateBoardReqBody
    {
        public string name;
        public string description;
        public string[] users_to_add;
        public string[] users_to_delete;

        public UpdateBoardReqBody(string name, string description, string[] usersToAdd, string[] usersToDelete)
        {
            this.name = name;
            this.description = description;
            users_to_add = usersToAdd;
            users_to_delete = usersToDelete;
        }
    }
}