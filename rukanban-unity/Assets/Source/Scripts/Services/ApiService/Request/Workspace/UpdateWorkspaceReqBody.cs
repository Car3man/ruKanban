namespace RuKanban.Services.Api.Request.Workspace
{
    public class UpdateWorkspaceReqBody
    {
        public string name;
        public string[] users_to_add;
        public string[] users_to_delete;

        public UpdateWorkspaceReqBody(string name, string[] usersToAdd, string[] usersToDelete)
        {
            this.name = name;
            users_to_add = usersToAdd;
            users_to_delete = usersToDelete;
        }
    }
}