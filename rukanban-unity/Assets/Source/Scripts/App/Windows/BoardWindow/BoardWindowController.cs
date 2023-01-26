using System;
using System.Collections.Generic;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api;
using RuKanban.Services.Api.DatabaseModels;
using RuKanban.Services.Api.Exceptions;
using RuKanban.Services.Api.Response.Column;
using RuKanban.Services.Api.Response.Ticket;
using UnityEngine;

namespace RuKanban.App.Window
{
    public class BoardWindowController : BaseAppWindowController
    {
        private readonly BoardWindow _window;

        private string _boardId;
        private List<Column> _boardColumns;
        private Dictionary<Column, List<Ticket>> _boardTickets;

        public BoardWindowController(AppManager appManager, BoardWindow window)
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }
        
        public void Reopen()
        {
            if (string.IsNullOrEmpty(_boardId))
            {
                throw new Exception($"Cant reopen {typeof(BoardWindow)} window cause '_boardId' is null");
            }
            
            Open(_boardId);
        }

        public async void Open(string boardId)
        {
            if (_window.IsActive())
            {
                _window.Hide(true, true);
            }

            _boardId = boardId;
            _boardColumns = new List<Column>();
            _boardTickets = new Dictionary<Column, List<Ticket>>();
            
            _window.Show(false);
            
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);
            
            ApiRequest getColumnsRequest = AppManager.ApiService.Column.GetColumns(boardId);
            HTTPResponse getColumnsResponse;

            try { getColumnsResponse = await AppManager.AuthorizedApiCall(this, getColumnsRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, getColumnsRequest, exception);
                return;
            }
            
            if (!getColumnsResponse.IsSuccess)
            {
                AppManager.OnUnexpectedApiCallException(this, getColumnsRequest, null);
                return;
            }

            loadingWindow.DestroyWindow();

            var getColumnsJsonResponse = JsonConvert.DeserializeObject<GetColumnsRes>(getColumnsResponse.DataAsText)!;

            _boardColumns = new List<Column>(getColumnsJsonResponse.columns);
            
            _window.header.backButton.onClick.AddListener(OnHeaderBackButtonClick);
            _window.header.settingsButton.onClick.AddListener(OnHeaderSettingsButtonClick);
            _window.OnColumnItemReady = OnColumnItemReady;
            _window.OnColumnTicketClick = OnColumnTicketClick;
            _window.OnColumnTicketMoveToAnotherColumn = OnColumnTicketMoveToAnotherColumn;
            _window.OnDeleteButtonClick = OnDeleteButtonClick;
            _window.OnAddTicketButtonClick = OnAddTicketButtonClick;
            _window.addColumnButton.onClick.AddListener(OnAddColumnButtonClick);
            _window.SetColumns(getColumnsJsonResponse.columns);
        }

        private void OnHeaderBackButtonClick()
        {
            _window.Hide(false, true);
            
            AppManager.GetReadyRootWindow<UserBoardsWindow, UserBoardsWindowController>().Reopen();
        }

        private void OnHeaderSettingsButtonClick()
        {
            AppManager.GetReadyRootWindow<SettingsWindow, SettingsWindowController>().Open();
        }

        private async void OnColumnItemReady(Column column, ColumnItem columnItem)
        {
            ApiRequest getTicketsRequest = AppManager.ApiService.Ticket.GetTickets(column.id);
            HTTPResponse getTicketsResponse;

            try { getTicketsResponse = await AppManager.AuthorizedApiCall(this, getTicketsRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, getTicketsRequest, exception);
                return;
            }
            
            if (!getTicketsResponse.IsSuccess)
            {
                AppManager.OnUnexpectedApiCallException(this, getTicketsRequest, null);
                return;
            }

            var getTicketsJsonResponse = JsonConvert.DeserializeObject<GetTicketsRes>(getTicketsResponse.DataAsText)!;
            
            _boardTickets.Add(column, new List<Ticket>(getTicketsJsonResponse.tickets));
            
            columnItem.SetTickets(new List<Ticket>(getTicketsJsonResponse.tickets));
        }

        private void OnAddColumnButtonClick()
        {
            AppManager.GetReadyRootWindow<CreateColumnWindow, CreateColumnWindowController>().Open(_boardId);
        }

        private void OnColumnTicketClick(Column column, Ticket ticket)
        {
            AppManager.GetReadyRootWindow<TicketWindow, TicketWindowController>().Open(ticket.id);
        }

        private async void OnDeleteButtonClick(Column column, ColumnItem columnItem)
        {
            ApiRequest deleteColumnRequest = AppManager.ApiService.Column.DeleteColumn(column.id);
            HTTPResponse deleteColumnResponse;

            try { deleteColumnResponse = await AppManager.AuthorizedApiCall(this, deleteColumnRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, deleteColumnRequest, exception);
                return;
            }
            
            if (!deleteColumnResponse.IsSuccess)
            {
                AppManager.OnUnexpectedApiCallException(this, deleteColumnRequest, null);
                return;
            }

            AppManager.GetReadyRootWindow<BoardWindow, BoardWindowController>().Reopen();
        }
        
        private void OnAddTicketButtonClick(Column column, ColumnItem columnItem)
        {
            AppManager.GetReadyRootWindow<CreateTicketWindow, CreateTicketWindowController>().Open(column.id);
        }

        private async void OnColumnTicketMoveToAnotherColumn(Column newColumn, Ticket ticket, int index)
        {
            ApiRequest moveTicketRequest = AppManager.ApiService.Ticket.MoveTicket(ticket.id, newColumn.id, index);
            HTTPResponse moveTicketResponse;
            
            try { moveTicketResponse = await AppManager.AuthorizedApiCall(this, moveTicketRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, moveTicketRequest, exception);
                return;
            }
            
            if (!moveTicketResponse.IsSuccess)
            {
                AppManager.OnUnexpectedApiCallException(this, moveTicketRequest, null);
                return;
            }
            
            AppManager.GetReadyRootWindow<BoardWindow, BoardWindowController>().Reopen();
        }
    }
}