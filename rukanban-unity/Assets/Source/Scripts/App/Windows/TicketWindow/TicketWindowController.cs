using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api;
using RuKanban.Services.Api.JsonModel;
using RuKanban.Services.Api.Responses;
using UnityEngine;

namespace RuKanban.App.Window
{
    public class TicketWindowController : BaseAppWindowController
    {
        private readonly TicketWindow _window;

        private string _ticketId;

        public TicketWindowController(AppManager appManager, TicketWindow window)
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }
        
        public async void Open(string ticketId)
        {
            if (_window.IsActive())
            {
                _window.Hide(true);
            }

            _ticketId = ticketId;
            
            _window.Show();
            
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);

            ApiRequest getTicketRequest = AppManager.ApiService.GetTicket(ticketId);
            HTTPResponse getTicketResponse;
            
            try { getTicketResponse = await AppManager.AuthorizedApiCall(this, getTicketRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, getTicketRequest, exception);
                return;
            }

            if (!getTicketResponse.IsSuccess)
            {
                AppManager.OnUnexpectedApiCallException(this, getTicketRequest, null);
                return;
            }

            loadingWindow.DestroyWindow();

            var getTicketJsonResponse = JsonConvert.DeserializeObject<Ticket>(getTicketResponse.DataAsText)!;

            _window.closeButton.onClick.AddListener(OnCloseButtonClick);
            _window.titleInput.text = getTicketJsonResponse.title;
            _window.descriptionInput.text = getTicketJsonResponse.description;
            _window.deleteButton.onClick.AddListener(OnTicketDeleteButtonClick);
            _window.saveButton.onClick.AddListener(OnTicketSaveButtonClick);
        }

        private void OnCloseButtonClick()
        {
            _window.Hide();
        }

        private async void OnTicketDeleteButtonClick()
        {
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);
            
            ApiRequest deleteTicketRequest = AppManager.ApiService.DeleteTicket(_ticketId);
            HTTPResponse deleteTicketResponse;

            try { deleteTicketResponse = await AppManager.AuthorizedApiCall(this, deleteTicketRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, deleteTicketRequest, exception);
                return;
            }

            if (!deleteTicketResponse.IsSuccess)
            {
                var deleteTicketJsonResponse = JsonConvert.DeserializeObject<BaseResponse>(deleteTicketResponse.DataAsText)!;
                
                if (!string.IsNullOrEmpty(deleteTicketJsonResponse.error_msg))
                {
                    loadingWindow.DestroyWindow();
                    
                    var messageBoxWindow = AppManager.CreateWindow<MessageBoxWindow>(AppManager.Windows.Root);
                    var messageBoxWindowController = new MessageBoxWindowController(AppManager, messageBoxWindow);
                    messageBoxWindowController.Open("Oops!", deleteTicketJsonResponse.error_msg, () =>
                    {
                        messageBoxWindow.DestroyWindow();
                    });
                    return;
                }
                
                AppManager.OnUnexpectedApiCallException(this, deleteTicketRequest, null);
                return;
            }
            
            loadingWindow.DestroyWindow();
            
            _window.Hide();
            
            AppManager.GetReadyRootWindow<BoardWindow, BoardWindowController>().Reopen();
        }

        private async void OnTicketSaveButtonClick()
        {
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);

            string newTitle = _window.titleInput.text;
            string newDescription = _window.descriptionInput.text;
            ApiRequest updateTicketRequest = AppManager.ApiService.UpdateTicket(_ticketId, null, null, newTitle, newDescription);
            HTTPResponse updateTicketResponse;

            try { updateTicketResponse = await AppManager.AuthorizedApiCall(this, updateTicketRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, updateTicketRequest, exception);
                return;
            }

            if (!updateTicketResponse.IsSuccess)
            {
                var updateTicketJsonResponse = JsonConvert.DeserializeObject<BaseResponse>(updateTicketResponse.DataAsText)!;
                
                if (!string.IsNullOrEmpty(updateTicketJsonResponse.error_msg))
                {
                    loadingWindow.DestroyWindow();
                    
                    var messageBoxWindow = AppManager.CreateWindow<MessageBoxWindow>(AppManager.Windows.Root);
                    var messageBoxWindowController = new MessageBoxWindowController(AppManager, messageBoxWindow);
                    messageBoxWindowController.Open("Oops!", updateTicketJsonResponse.error_msg, () =>
                    {
                        messageBoxWindow.DestroyWindow();
                    });
                    return;
                }
                
                AppManager.OnUnexpectedApiCallException(this, updateTicketRequest, null);
                return;
            }
            
            loadingWindow.DestroyWindow();
            
            _window.Hide();
            
            AppManager.GetReadyRootWindow<BoardWindow, BoardWindowController>().Reopen();
        }
    }
}