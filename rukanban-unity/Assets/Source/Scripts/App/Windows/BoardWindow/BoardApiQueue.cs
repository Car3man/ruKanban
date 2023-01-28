using System;
using System.Collections.Generic;
using System.Threading;
using BestHTTP;
using RuKanban.Services.Api;

namespace RuKanban.App.Window
{
    public class BoardApiQueue : IDisposable
    {
        private readonly AppManager _appManager;
        private readonly BoardWindowController _boardWindowController;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly List<(ApiRequest, Action<ApiRequest, HTTPResponse>)> _queue;
        
        private ApiRequest _currProcessingRequest;
        
        public BoardApiQueue(AppManager appManager, BoardWindowController boardWindowController)
        {
            _appManager = appManager;
            _boardWindowController = boardWindowController;

            _cancellationTokenSource = new CancellationTokenSource();
            _queue = new List<(ApiRequest, Action<ApiRequest, HTTPResponse>)>();
            
            _currProcessingRequest = null;
        }

        public async void Tick()
        {
            if (_currProcessingRequest == null && _queue.Count > 0)
            {
                var (request, callback) = _queue[0];
                _queue.RemoveAt(0);
                
                _currProcessingRequest = request;
                
                HTTPResponse response = await _appManager.ApiCall(_boardWindowController, _currProcessingRequest);
                callback?.Invoke(_currProcessingRequest, response);

                _currProcessingRequest = null;
            }
        }

        public void CallApi(ApiRequest request, Action<ApiRequest, HTTPResponse> callback)
        {
            request.SetCancellationToken(_cancellationTokenSource.Token);
            _queue.Add((request, callback));
        }

        public bool IsAnyRequest()
        {
            return _currProcessingRequest != null || _queue.Count > 0;
        }

        public void CancelAndClear()
        {
            _cancellationTokenSource.Cancel();
            _queue.Clear();
            _currProcessingRequest = null;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }
    }
}