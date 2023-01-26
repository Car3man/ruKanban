using System;

namespace RuKanban.Services.Api.DatabaseModels
{
    [Serializable]
    public class Workspace
    {
        public string id;
        public string name;
        public string created_at;
    }
}