using System;

namespace RuKanban.Services.Api.RequestJsonBody
{
    [Serializable]
    public class UpdateWorkspaceRequest
    {
        public string name;
        public string[] users_to_add;
        public string[] users_to_delete;

        public UpdateWorkspaceRequest(string name, string[] usersToAdd, string[] usersToDelete)
        {
            this.name = name;
            users_to_add = usersToAdd;
            users_to_delete = usersToDelete;
        }
    }
}