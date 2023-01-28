using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly BoardApiQueue _apiQueue;

        private string _id;
        private Dictionary<ColumnItem, Column> _columnsBindings;
        private Dictionary<TicketItem, Ticket> _ticketsBindings;
        private Dictionary<ColumnItem, List<TicketItem>> _columnTicketBindings;

        public BoardWindowController(AppManager appManager, BoardWindow window)
            : base(appManager, window)
        {
            appManager.MakeWindowControllerTickable(this);
            
            _window = window;
            _window.BindController(this);
            
            _apiQueue = new BoardApiQueue(appManager, this);
        }

        public override void Tick()
        {
            _apiQueue.Tick();
        }

        public void Reopen()
        {
            if (string.IsNullOrEmpty(_id))
            {
                throw new Exception($"Cant reopen {typeof(BoardWindow)} window because '_id' is null");
            }
            
            Open(_id);
        }

        public async void Open(string boardId)
        {
            if (_window.IsActive())
            {
                _window.Hide(true, true);
            }

            _id = boardId;
            _columnsBindings = new Dictionary<ColumnItem, Column>();
            _ticketsBindings = new Dictionary<TicketItem, Ticket>();
            _columnTicketBindings = new Dictionary<ColumnItem, List<TicketItem>>();
            
            _window.Show(false);
            
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);
            
            ApiRequest getColumnsRequest = AppManager.ApiService.Column.GetColumns(boardId);
            HTTPResponse getColumnsResponse;

            try { getColumnsResponse = await AppManager.ApiCall(this, getColumnsRequest); }
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
            
            _window.header.backButton.onClick.AddListener(OnHeaderBackButtonClick);
            _window.header.settingsButton.onClick.AddListener(OnHeaderSettingsButtonClick);
            _window.addColumnButton.onClick.AddListener(OnAddColumnButtonClick);
            _window.OnColumnItemReady = OnColumnItemReady;
            _window.OnColumnDeleteButtonClick = OnColumnDeleteButtonClick;
            _window.OnColumnTicketClick = OnColumnTicketClick;
            _window.OnColumnTicketMove = OnColumnTicketMove;
            _window.OnColumnAddTicketButtonClick = OnColumnAddTicketButtonClick;
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

        private void OnAddColumnButtonClick()
        {
            AppManager.GetReadyRootWindow<CreateColumnWindow, CreateColumnWindowController>().Open(_window.CreateColumnLocal);
        }

        private async void OnColumnItemReady(ColumnItem columnItem, Column column, bool isLocal)
        {
            _columnsBindings.Add(columnItem, column);
            _columnTicketBindings.Add(columnItem, new List<TicketItem>());
            
            columnItem.OnTicketItemReady = (ticketItem, ticket, isTicketLocal) =>
            {
                OnTicketItemReady(columnItem, ticketItem, ticket, isTicketLocal);
            };
            
            if (isLocal)
            {
                ApiRequest createColumnRequest = AppManager.ApiService.Column.CreateColumn(_id, column.title);
                _apiQueue.CallApi(createColumnRequest, (request, response) =>
                {
                    CreateRemoteColumnResponse(columnItem, request, response);
                });
            }
            else
            {
                ApiRequest getTicketsRequest = AppManager.ApiService.Ticket.GetTickets(column.id);
                HTTPResponse getTicketsHTTPResponse = await AppManager.ApiCall(this, getTicketsRequest);

                if (!getTicketsHTTPResponse.IsSuccess)
                {
                    AppManager.OnUnexpectedApiCallException(this, getTicketsRequest, null);
                    return;
                }

                var getTicketsResponse = JsonConvert.DeserializeObject<GetTicketsRes>(getTicketsHTTPResponse.DataAsText)!;
                columnItem.SetTickets(new List<Ticket>(getTicketsResponse.tickets));
            }
        }

        private void CreateRemoteColumnResponse(ColumnItem columnItem, ApiRequest request, HTTPResponse httpResponse)
        {
            var createColumnResponse = JsonConvert.DeserializeObject<CreateColumnRes>(httpResponse.DataAsText)!;
            if (!httpResponse.IsSuccess)
            {
                _apiQueue.CancelAndClear();
                AppManager.OnUnexpectedApiCallException(this, request, null);
                return;
            }

            _columnsBindings[columnItem] = createColumnResponse.column;
        }

        private void OnTicketItemReady(ColumnItem columnItem, TicketItem ticketItem, Ticket ticket, bool isLocal)
        {
            _ticketsBindings.Add(ticketItem, ticket);
            _columnTicketBindings[columnItem].Add(ticketItem);
            
            if (isLocal)
            {
                Column column = _columnsBindings[columnItem];
                ApiRequest createTicketRequest = AppManager.ApiService.Ticket.CreateTicket(column.id, ticket.title, ticket.description);
                _apiQueue.CallApi(createTicketRequest, (request, response) =>
                {
                    CreateRemoteTicketResponse(ticketItem, request, response);
                });
            }
        }

        private void CreateRemoteTicketResponse(TicketItem ticketItem, ApiRequest request, HTTPResponse httpResponse)
        {
            var createTicketResponse = JsonConvert.DeserializeObject<CreateTicketRes>(httpResponse.DataAsText)!;
            if (!httpResponse.IsSuccess)
            {
                _apiQueue.CancelAndClear();
                AppManager.OnUnexpectedApiCallException(this, request, null);
                return;
            }

            _ticketsBindings[ticketItem] = createTicketResponse.ticket;
        }

        private void OnColumnDeleteButtonClick(ColumnItem columnItem)
        {
            _window.DeleteColumn(columnItem);
            
            Column column = _columnsBindings[columnItem];
            ApiRequest deleteColumnRequest = AppManager.ApiService.Column.DeleteColumn(column.id);
            _apiQueue.CallApi(deleteColumnRequest, (request, httpResponse) =>
            {
                if (!httpResponse.IsSuccess)
                {
                    _apiQueue.CancelAndClear();
                    AppManager.OnUnexpectedApiCallException(this, request, null);
                    return;
                }
            });
        }

        private void OnColumnAddTicketButtonClick(ColumnItem columnItem)
        {
            AppManager.GetReadyRootWindow<CreateTicketWindow, CreateTicketWindowController>().Open(columnItem.CreateTicketLocal);
        }

        private void OnColumnTicketClick(TicketItem ticketItem)
        {
            Ticket ticket = _ticketsBindings[ticketItem];

            AppManager.GetReadyRootWindow<TicketWindow, TicketWindowController>().Open(ticket.id);
        }

        private void OnColumnTicketMove(ColumnItem oldColumnItem, TicketItem ticketItem, ColumnItem newColumnItem, TicketItem insertAfterItem)
        {
            Ticket ticketToMove = _ticketsBindings[ticketItem];
            Column moveToColumn = _columnsBindings[newColumnItem];
            Ticket insertAfterTicket = insertAfterItem != null ? _ticketsBindings[insertAfterItem] : null;
            
            string ticketId = ticketToMove.id;
            string columnId = moveToColumn.id;
            int ticketIndex = insertAfterTicket != null ? insertAfterTicket.index + 1 : 0;

            _columnTicketBindings[oldColumnItem].Remove(ticketItem);
            int insertIndex = insertAfterItem != null ? _columnTicketBindings[newColumnItem].IndexOf(insertAfterItem) + 1 : 0;
            List<Ticket> ticketsToUpdateIndex = _columnTicketBindings[newColumnItem]
                .Select(x => _ticketsBindings[x])
                .Where(x => x.index >= ticketIndex)
                .OrderByDescending(x => x.index)
                .ToList();
            for (int i = 0; i < ticketsToUpdateIndex.Count; i++)
            {
                ticketsToUpdateIndex[i].index = ticketIndex + (ticketsToUpdateIndex.Count - i);
            }
            _columnTicketBindings[newColumnItem].Insert(insertIndex, ticketItem);
            ticketToMove.column_id = columnId;
            ticketToMove.index = ticketIndex;
            newColumnItem.TakeTicket(ticketItem, insertAfterItem);
            
            ApiRequest moveTicketRequest = AppManager.ApiService.Ticket.MoveTicket(ticketId, columnId, ticketIndex);
            _apiQueue.CallApi(moveTicketRequest, (request, httpResponse) =>
            {
                if (!httpResponse.IsSuccess)
                {
                    _apiQueue.CancelAndClear();
                    AppManager.OnUnexpectedApiCallException(this, request, null);
                    return;
                }
            });
        }
    }
}