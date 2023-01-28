using System;
using System.Collections.Generic;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api;
using RuKanban.Services.Api.DatabaseModels;
using RuKanban.Services.Api.Exceptions;
using RuKanban.Services.Api.Response.Workspace;

namespace RuKanban.App.Window
{
    public class WorkspaceSettingsWindowController : BaseAppWindowController
    {
        private readonly WorkspaceSettingsWindow _window;

        private string _workspaceId;

        public WorkspaceSettingsWindowController(AppManager appManager, WorkspaceSettingsWindow window)
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }
        
        public async void Open(string workspaceId)
        {
            if (_window.IsActive())
            {
                _window.Hide(true, true);
            }

            _workspaceId = workspaceId;
            
            _window.Show(false);
            
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);

            ApiRequest getWorkspaceByIdRequest = AppManager.ApiService.Workspace.GetWorkspaceById(workspaceId);
            ApiRequest getWorkspaceUsersRequest = AppManager.ApiService.Workspace.GetWorkspaceUsers(workspaceId);
            HTTPResponse getWorkspaceByIdResponse;
            HTTPResponse getWorkspaceUsersResponse;
            
            try { getWorkspaceByIdResponse = await AppManager.ApiCall(this, getWorkspaceByIdRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, getWorkspaceByIdRequest, exception);
                return;
            }
            
            try { getWorkspaceUsersResponse = await AppManager.ApiCall(this, getWorkspaceUsersRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, getWorkspaceUsersRequest, exception);
                return;
            }

            if (!getWorkspaceByIdResponse.IsSuccess || !getWorkspaceUsersResponse.IsSuccess)
            {
                AppManager.OnUnexpectedApiCallException(this, getWorkspaceUsersRequest, null);
                return;
            }

            loadingWindow.DestroyWindow();

            var getWorkspaceByIdJsonResponse = JsonConvert.DeserializeObject<GetWorkspaceByIdRes>(getWorkspaceByIdResponse.DataAsText)!;
            var getWorkspaceUsersJsonResponse = JsonConvert.DeserializeObject<GetWorkspaceUsersRes>(getWorkspaceUsersResponse.DataAsText)!;

            _window.closeButton.onClick.AddListener(OnCloseButtonClick);
            _window.nameInput.text = getWorkspaceByIdJsonResponse.workspace.name;
            _window.nameInput.onValueChanged.AddListener(OnNameInputChange);
            _window.SetWorkspaceUsers(getWorkspaceUsersJsonResponse.users);
            _window.OnUserItemDeleteButtonClick = OnUserItemDeleteButtonClick;
            _window.inviteUserButton.onClick.AddListener(OnInviteUserButtonClick);
        }

        private void OnCloseButtonClick()
        {
            _window.Hide(false, true);
        }

        private void OnNameInputChange(string newName)
        {
            // TODO
        }

        private async void OnUserItemDeleteButtonClick(User user)
        {
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);

            string userToDelete = user.login;
            ApiRequest updateWorkspaceRequest = AppManager.ApiService.Workspace
                .UpdateWorkspace(_workspaceId, null, null, new [] {userToDelete});
            HTTPResponse updateWorkspaceResponse;

            try { updateWorkspaceResponse = await AppManager.ApiCall(this, updateWorkspaceRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, updateWorkspaceRequest, exception);
                return;
            }

            if (!updateWorkspaceResponse.IsSuccess)
            {
                var updateWorkspaceJsonResponse = JsonConvert.DeserializeObject<UpdateWorkspaceRes>(updateWorkspaceResponse.DataAsText)!;
                
                if (!string.IsNullOrEmpty(updateWorkspaceJsonResponse.error_msg))
                {
                    loadingWindow.DestroyWindow();
                    
                    var messageBoxWindow = AppManager.CreateWindow<MessageBoxWindow>(AppManager.Windows.Root);
                    var messageBoxWindowController = new MessageBoxWindowController(AppManager, messageBoxWindow);
                    messageBoxWindowController.Open("Oops!", updateWorkspaceJsonResponse.error_msg, () =>
                    {
                        messageBoxWindow.DestroyWindow();
                    });
                    return;
                }
                
                AppManager.OnUnexpectedApiCallException(this, updateWorkspaceRequest, null);
                return;
            }
            
            loadingWindow.DestroyWindow();
            
            Open(_workspaceId);
        }

        private async void OnInviteUserButtonClick()
        {
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);

            string userToAdd = _window.inviteUserLoginInput.text;
            ApiRequest updateWorkspaceRequest = AppManager.ApiService.Workspace
                .UpdateWorkspace(_workspaceId, null, new [] {userToAdd}, null);
            HTTPResponse updateWorkspaceResponse;

            try { updateWorkspaceResponse = await AppManager.ApiCall(this, updateWorkspaceRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, updateWorkspaceRequest, exception);
                return;
            }

            if (!updateWorkspaceResponse.IsSuccess)
            {
                var updateWorkspaceJsonResponse = JsonConvert.DeserializeObject<UpdateWorkspaceRes>(updateWorkspaceResponse.DataAsText)!;
                
                if (!string.IsNullOrEmpty(updateWorkspaceJsonResponse.error_msg))
                {
                    loadingWindow.DestroyWindow();
                    
                    var messageBoxWindow = AppManager.CreateWindow<MessageBoxWindow>(_window);
                    var messageBoxWindowController = new MessageBoxWindowController(AppManager, messageBoxWindow);
                    messageBoxWindowController.Open("Oops!", updateWorkspaceJsonResponse.error_msg, () =>
                    {
                        messageBoxWindow.DestroyWindow();
                    });
                    return;
                }
                
                AppManager.OnUnexpectedApiCallException(this, updateWorkspaceRequest, null);
                return;
            }
            
            loadingWindow.DestroyWindow();
            
            Open(_workspaceId);
        }
    }
}