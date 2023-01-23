using System;

namespace RuKanban.Services.Api.RequestJsonBody
{
    [Serializable]
    public class CreateColumnRequest
    {
        public string name;

        public CreateColumnRequest(string name)
        {
            this.name = name;
        }
    }
}