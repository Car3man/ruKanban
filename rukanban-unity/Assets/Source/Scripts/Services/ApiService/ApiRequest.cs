using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BestHTTP;

namespace RuKanban.Services.Api
{
    public class ApiRequest
    {
        private Uri _uri;
        private HTTPMethods _method;
        private Dictionary<string, string> _headers;
        private byte[] _rawData;

        public ApiRequest(Uri uri, HTTPMethods method)
        {
            _uri = uri;
            _method = method;
            _headers = new Dictionary<string, string>();
            _rawData = Array.Empty<byte>();
        }

        public void AddHeader(string name, string value)
        {
            if (_headers.ContainsKey(name))
            {
                _headers[name] = value;
            }
            else
            {
                _headers.Add(name, value);
            }
        }

        public void SetJsonBody(string json)
        {
            AddHeader("Content-type", "application/json; charset=UTF-8");
            _rawData = Encoding.UTF8.GetBytes(json);
        }

        public Task<HTTPResponse> GetHTTPResponseAsync()
        {
            var request = new HTTPRequest(_uri, _method);
            foreach (string header in _headers.Keys)
            {
                request.AddHeader(header, _headers[header]);
            }
            request.RawData = _rawData;
            return request.GetHTTPResponseAsync();
        }
    }
}