using System;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api;
using RuKanban.Services.Api.Exceptions;
using RuKanban.Services.Api.Response.Workspace;

namespace RuKanban.App.Window
{
    public class CreateUserWorkspaceWindowController : BaseAppWindowController
    {
        private readonly CreateUserWorkspaceWindow _window;

        public CreateUserWorkspaceWindowController(AppManager appManager, CreateUserWorkspaceWindow window)
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }

        public void Open()
        {
            if (_window.IsActive())
            {
                _window.Hide(true, true);
            }
            
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

            string workspaceName = _window.nameInput.text;
            ApiRequest createWorkspaceRequest = AppManager.ApiService.Workspace.CreateWorkspace(workspaceName);
            HTTPResponse createWorkspaceResponse;

            try { createWorkspaceResponse = await AppManager.ApiCall(this, createWorkspaceRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, createWorkspaceRequest, exception);
                return;
            }

            var createWorkspaceJsonResponse = JsonConvert.DeserializeObject<CreateWorkspaceRes>(createWorkspaceResponse.DataAsText)!;
            if (!createWorkspaceResponse.IsSuccess)
            {
                loadingWindow.DestroyWindow();
                
                if (!string.IsNullOrEmpty(createWorkspaceJsonResponse.error_msg))
                {
                    var messageBoxWindow = AppManager.CreateWindow<MessageBoxWindow>(_window);
                    var messageBoxWindowController = new MessageBoxWindowController(AppManager, messageBoxWindow);
                    messageBoxWindowController.Open("Oops!", createWorkspaceJsonResponse.error_msg, () =>
                    {
                        messageBoxWindow.DestroyWindow();
                    });
                    return;
                }
                
                AppManager.OnUnexpectedApiCallException(this, createWorkspaceRequest, null);
                return;
            }
            
            loadingWindow.DestroyWindow();
            
            _window.Hide(false, true);
            AppManager.GetReadyRootWindow<UserWorkspacesWindow, UserWorkspacesWindowController>().Open();
        }
    }
}