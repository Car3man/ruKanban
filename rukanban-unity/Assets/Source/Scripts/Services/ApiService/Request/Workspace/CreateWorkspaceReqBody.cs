namespace RuKanban.Services.Api.Request.Workspace
{
    public class CreateWorkspaceReqBody
    {
        public string name;

        public CreateWorkspaceReqBody(string name)
        {
            this.name = name;
        }
    }
}