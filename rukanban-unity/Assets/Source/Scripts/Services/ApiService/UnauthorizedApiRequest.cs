using System;

namespace RuKanban.Services.Api
{
    public class UnauthorizedApiRequest : Exception
    {
        public UnauthorizedApiRequest()
        {
        }

        public UnauthorizedApiRequest(string message) : base(message)
        {
        }
    }
}