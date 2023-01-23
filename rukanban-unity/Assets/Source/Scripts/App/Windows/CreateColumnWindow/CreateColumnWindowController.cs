using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api;
using RuKanban.Services.Api.Responses;
using UnityEngine;

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
                _window.Hide(true);
            }
            
            _boardId = boardId;
            
            _window.Show();
            _window.closeButton.onClick.AddListener(OnCloseButtonClick);
            _window.createButton.onClick.AddListener(OnCreateButtonClick);
        }

        private void OnCloseButtonClick()
        {
            _window.Hide();
        }

        private async void OnCreateButtonClick()
        {
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);

            string columnName = _window.nameInput.text;
            ApiRequest createColumnRequest = AppManager.ApiService.CreateColumn(_boardId, columnName);
            HTTPResponse createColumnResponse;
            
            try { createColumnResponse = await AppManager.AuthorizedApiCall(this, createColumnRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, createColumnRequest, exception);
                return;
            }
            
            var createColumnJsonResponse = JsonConvert.DeserializeObject<CreateColumnResponse>(createColumnResponse.DataAsText)!;
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
            
            _window.Hide();
            AppManager.GetReadyRootWindow<BoardWindow, BoardWindowController>().Reopen();
        }
    }
}