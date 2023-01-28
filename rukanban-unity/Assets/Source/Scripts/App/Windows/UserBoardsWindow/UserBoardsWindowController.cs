using System;
using System.Collections.Generic;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api;
using RuKanban.Services.Api.DatabaseModels;
using RuKanban.Services.Api.Exceptions;
using RuKanban.Services.Api.Response.Board;
using UnityEngine;

namespace RuKanban.App.Window
{
    public class UserBoardsWindowController : BaseAppWindowController
    {
        private readonly UserBoardsWindow _window;
        private string _workspaceId;
        
        public UserBoardsWindowController(AppManager appManager, UserBoardsWindow window) 
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }

        public void Reopen()
        {
            if (string.IsNullOrEmpty(_workspaceId))
            {
                throw new Exception($"Cant reopen {typeof(UserBoardsWindow)} window cause '_workspaceId' is null");
            }
            
            Open(_workspaceId);
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
            
            ApiRequest getBoardsRequest = AppManager.ApiService.Board.GetBoards(workspaceId);
            HTTPResponse getBoardsResponse;

            try { getBoardsResponse = await AppManager.ApiCall(this, getBoardsRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, getBoardsRequest, exception);
                return;
            }
            
            if (!getBoardsResponse.IsSuccess)
            {
                AppManager.OnUnexpectedApiCallException(this, getBoardsRequest, null);
                return;
            }
            
            loadingWindow.DestroyWindow();
            
            var getBoardsJsonResponse = JsonConvert.DeserializeObject<GetBoardsRes>(getBoardsResponse.DataAsText)!;

            _window.header.backButton.onClick.AddListener(OnHeaderBackButtonClick);
            _window.header.settingsButton.onClick.AddListener(OnHeaderSettingsButtonClick); 
            _window.addUserBoardButton.onClick.AddListener(OnAddUserBoardButtonClick);
            _window.OnUserBoardItemButtonClick = OnUserBoardItemClick;
            _window.SetUserBoards(new List<Board>(getBoardsJsonResponse.boards));
        }

        private void OnHeaderBackButtonClick()
        {
            _window.Hide(false, true);
            
            AppManager.GetReadyRootWindow<UserWorkspacesWindow, UserWorkspacesWindowController>().Open();
        }
        
        private void OnHeaderSettingsButtonClick()
        {
            AppManager.GetReadyRootWindow<SettingsWindow, SettingsWindowController>().Open();
        }

        private void OnUserBoardItemClick(Board userBoard)
        {
            AppManager.GetReadyRootWindow<BoardWindow, BoardWindowController>().Open(userBoard.id);
        }

        private void OnAddUserBoardButtonClick()
        {
            AppManager.GetReadyRootWindow<CreateUserBoardWindow, CreateUserBoardWindowController>().Open(_workspaceId);
        }
    }
}