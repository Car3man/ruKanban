using System;
using System.Collections.Generic;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api;
using RuKanban.Services.Api.JsonModel;

namespace RuKanban.App.Window
{
    public class UserWorkspacesWindowController : BaseAppWindowController
    {
        private readonly UserWorkspacesWindow _window;

        public UserWorkspacesWindowController(AppManager appManager, UserWorkspacesWindow window) 
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }

        public async void Open()
        {
            if (_window.IsActive())
            {
                _window.Hide(true);
            }
            
            _window.Show();
            
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);
            
            ApiRequest getWorkspacesRequest = AppManager.ApiService.GetWorkspaces();
            HTTPResponse getWorkspacesResponse;

            try { getWorkspacesResponse = await AppManager.AuthorizedApiCall(this, getWorkspacesRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, getWorkspacesRequest, exception);
                return;
            }
            
            if (!getWorkspacesResponse.IsSuccess)
            {
                AppManager.OnUnexpectedApiCallException(this, getWorkspacesRequest, null);
                return;
            }

            loadingWindow.DestroyWindow();

            var getWorkspacesJsonResponse = JsonConvert.DeserializeObject<List<Workspace>>(getWorkspacesResponse.DataAsText)!;
            
            _window.header.settingsButton.onClick.AddListener(OnHeaderSettingsButtonClick); 
            _window.OnUserWorkspaceItemButtonClick = OnUserWorkspaceItemClick;
            _window.OnUserWorkspaceItemSettingsButtonClick = OnUserWorkspaceItemSettingsButtonClick;
            _window.addUserWorkspaceButton.onClick.AddListener(OnAddUserWorkspaceButtonClick);
            _window.SetUserWorkspaces(getWorkspacesJsonResponse);
        }

        private void OnHeaderSettingsButtonClick()
        {
            AppManager.GetReadyRootWindow<SettingsWindow, SettingsWindowController>().Open();
        }
        
        private void OnUserWorkspaceItemClick(Workspace userWorkspace)
        {
            _window.Hide();
            AppManager.GetReadyRootWindow<UserBoardsWindow, UserBoardsWindowController>().Open(userWorkspace.id);
        }
        
        private void OnUserWorkspaceItemSettingsButtonClick(Workspace userWorkspace)
        {
            AppManager.GetReadyRootWindow<WorkspaceSettingsWindow, WorkspaceSettingsWindowController>().Open(userWorkspace.id);
        }

        private void OnAddUserWorkspaceButtonClick()
        {
            AppManager.GetReadyRootWindow<CreateUserWorkspaceWindow, CreateUserWorkspaceWindowController>().Open();
        }
    }
}