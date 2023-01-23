using System;

namespace RuKanban.Services.Api.RequestJsonBody
{
    [Serializable]
    public class CreateWorkspaceRequest
    {
        public string name;

        public CreateWorkspaceRequest(string name)
        {
            this.name = name;
        }
    }
}