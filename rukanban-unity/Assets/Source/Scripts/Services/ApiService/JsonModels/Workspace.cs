using System;

namespace RuKanban.Services.Api.JsonModel
{
    [Serializable]
    public class Workspace
    {
        public string id;
        public string name;
        public string created_at;
    }
}