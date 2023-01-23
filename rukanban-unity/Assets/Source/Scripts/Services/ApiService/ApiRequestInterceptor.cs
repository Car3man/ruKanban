using System;
using BestHTTP;

namespace RuKanban.Services.Api
{
    public class ApiRequestInterceptor
    {
        private readonly Func<ApiRequest, ApiRequest> _interceptFunc;
    
        public ApiRequestInterceptor(Func<ApiRequest, ApiRequest> interceptFunc)
        {
            _interceptFunc = interceptFunc;
        }

        public ApiRequest Intercept(ApiRequest request)
        {
            return _interceptFunc.Invoke(request);
        }
    }
}