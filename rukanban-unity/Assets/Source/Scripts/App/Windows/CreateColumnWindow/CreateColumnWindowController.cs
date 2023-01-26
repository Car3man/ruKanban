using System;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api;
using RuKanban.Services.Api.Exceptions;
using RuKanban.Services.Api.Response.Column;

namespace RuKanban.App.Window
{
    public class CreateColumnWindowController : BaseAppWindowController
    {
        private readonly CreateColumnWindow _window;
        private string _boardId;

        public CreateColumnWindowController(AppManager appManager, CreateColumnWindow window)
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }

        public void Open(string boardId)
        {
            if (_window.IsActive())
            {
                _window.Hide(true, true);
            }
            
            _boardId = boardId;
            
            _window.Show(false);
            _window.closeButton.onClick.AddListener(OnCloseButtonClick);
            _window.createButton.onClick.AddListener(OnCreateButtonClick);
        }

        private void OnCloseButtonClick()
        {
            _window.Hide(false, true);
        }

        private async void OnCreateButtonClick()
        {
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);

            string columnName = _window.nameInput.text;
            ApiRequest createColumnRequest = AppManager.ApiService.Column.CreateColumn(_boardId, columnName);
            HTTPResponse createColumnResponse;
            
            try { createColumnResponse = await AppManager.AuthorizedApiCall(this, createColumnRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, createColumnRequest, exception);
                return;
            }
            
            var createColumnJsonResponse = JsonConvert.DeserializeObject<CreateColumnRes>(createColumnResponse.DataAsText)!;
            if (!createColumnResponse.IsSuccess)
            {
                if (!string.IsNullOrEmpty(createColumnJsonResponse.error_msg))
                {
                    loadingWindow.DestroyWindow();
                    
                    var messageBoxWindow = AppManager.CreateWindow<MessageBoxWindow>(_window);
                    var messageBoxWindowController = new MessageBoxWindowController(AppManager, messageBoxWindow);
                    messageBoxWindowController.Open("Oops!", createColumnJsonResponse.error_msg, () =>
                    {
                        messageBoxWindow.DestroyWindow();
                    });
                    return;
                }
                
                AppManager.OnUnexpectedApiCallException(this, createColumnRequest, null);
                return;
            }
            
            loadingWindow.DestroyWindow();
            
            _window.Hide(false, true);
            AppManager.GetReadyRootWindow<BoardWindow, BoardWindowController>().Reopen();
        }
    }
}