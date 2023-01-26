namespace RuKanban.Services.Api.Response.Workspace
{
    public class GetWorkspaceUsersRes : BaseRes
    {
        public DatabaseModels.User[] users;
    }
}