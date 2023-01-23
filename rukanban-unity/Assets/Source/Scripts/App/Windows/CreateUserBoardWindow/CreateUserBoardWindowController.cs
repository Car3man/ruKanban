using System;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api;
using RuKanban.Services.Api.Responses;

namespace RuKanban.App.Window
{
    public class CreateUserBoardWindowController : BaseAppWindowController
    {
        private readonly CreateUserBoardWindow _window;
        private string _workspaceId;

        public CreateUserBoardWindowController(AppManager appManager, CreateUserBoardWindow window)
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }

        public void Open(string workspaceId)
        {
            if (_window.IsActive())
            {
                _window.Hide(true);
            }
            
            _workspaceId = workspaceId;
            
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

            string boardName = _window.nameInput.text;
            string boardDescription = _window.descriptionInput.text;
            ApiRequest createBoardRequest = AppManager.ApiService.CreateBoard(_workspaceId, boardName, boardDescription);
            HTTPResponse createBoardResponse;
            
            try { createBoardResponse = await AppManager.AuthorizedApiCall(this, createBoardRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, createBoardRequest, exception);
                return;
            }
            
            var createBoardJsonResponse = JsonConvert.DeserializeObject<CreateBoardResponse>(createBoardResponse.DataAsText)!;
            if (!createBoardResponse.IsSuccess)
            {
                if (!string.IsNullOrEmpty(createBoardJsonResponse.error_msg))
                {
                    loadingWindow.DestroyWindow();
                    
                    var messageBoxWindow = AppManager.CreateWindow<MessageBoxWindow>(_window);
                    var messageBoxWindowController = new MessageBoxWindowController(AppManager, messageBoxWindow);
                    messageBoxWindowController.Open("Oops!", createBoardJsonResponse.error_msg, () =>
                    {
                        messageBoxWindow.DestroyWindow();
                    });
                    return;
                }
                
                AppManager.OnUnexpectedApiCallException(this, createBoardRequest, null);
                return;
            }
            
            loadingWindow.DestroyWindow();
            
            _window.Hide();
            AppManager.GetReadyRootWindow<UserBoardsWindow, UserBoardsWindowController>().Open(createBoardJsonResponse.workspace_id);
        }
    }
}