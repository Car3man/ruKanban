using System;

namespace RuKanban.Services.Api.Routes
{
    public class BaseRoute
    {
        private readonly string _basePath;

        public BaseRoute(string basePath)
        {
            _basePath = basePath;
        }

        protected Uri GetUri(string route)
        {
            return new Uri($"{_basePath}{route}");
        }
    }
}