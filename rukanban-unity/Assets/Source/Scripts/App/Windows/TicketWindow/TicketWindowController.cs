using System;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api;
using RuKanban.Services.Api.Exceptions;
using RuKanban.Services.Api.Response.Ticket;

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
                _window.Hide(true, true);
            }

            _ticketId = ticketId;
            
            _window.Show(false);
            
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);

            ApiRequest getTicketByIdRequest = AppManager.ApiService.Ticket.GetTicketById(ticketId);
            HTTPResponse getTicketByIdResponse;
            
            try { getTicketByIdResponse = await AppManager.ApiCall(this, getTicketByIdRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, getTicketByIdRequest, exception);
                return;
            }

            if (!getTicketByIdResponse.IsSuccess)
            {
                AppManager.OnUnexpectedApiCallException(this, getTicketByIdRequest, null);
                return;
            }

            loadingWindow.DestroyWindow();

            var getTicketByIdJsonResponse = JsonConvert.DeserializeObject<GetTicketByIdRes>(getTicketByIdResponse.DataAsText)!;

            _window.closeButton.onClick.AddListener(OnCloseButtonClick);
            _window.titleInput.text = getTicketByIdJsonResponse.ticket.title;
            _window.descriptionInput.text = getTicketByIdJsonResponse.ticket.description;
            _window.deleteButton.onClick.AddListener(OnTicketDeleteButtonClick);
            _window.saveButton.onClick.AddListener(OnTicketSaveButtonClick);
        }

        private void OnCloseButtonClick()
        {
            _window.Hide(false, true);
        }

        private async void OnTicketDeleteButtonClick()
        {
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);
            
            ApiRequest deleteTicketRequest = AppManager.ApiService.Ticket.DeleteTicket(_ticketId);
            HTTPResponse deleteTicketResponse;

            try { deleteTicketResponse = await AppManager.ApiCall(this, deleteTicketRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, deleteTicketRequest, exception);
                return;
            }

            if (!deleteTicketResponse.IsSuccess)
            {
                var deleteTicketJsonResponse = JsonConvert.DeserializeObject<DeleteTicketRes>(deleteTicketResponse.DataAsText)!;
                
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
            
            _window.Hide(false, true);
            
            // TODO: do local changes, instead force reopen board window
            AppManager.GetReadyRootWindow<BoardWindow, BoardWindowController>().Reopen();
        }

        private async void OnTicketSaveButtonClick()
        {
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);
            
            string newTitle = _window.titleInput.text;
            string newDescription = _window.descriptionInput.text;
            
            ApiRequest changeTicketTitleRequest = AppManager.ApiService.Ticket.ChangeTitle(_ticketId, newTitle);
            ApiRequest changeTicketDescriptionRequest = AppManager.ApiService.Ticket.ChangeDescription(_ticketId, newDescription);
            
            HTTPResponse changeTicketTitleResponse;
            HTTPResponse changeTicketDescriptionResponse;
            
            try { changeTicketTitleResponse = await AppManager.ApiCall(this, changeTicketTitleRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, changeTicketTitleRequest, exception);
                return;
            }
            
            try { changeTicketDescriptionResponse = await AppManager.ApiCall(this, changeTicketDescriptionRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, changeTicketDescriptionRequest, exception);
                return;
            }
            
            if (!changeTicketTitleResponse.IsSuccess)
            {
                var changeTicketTitleJsonResponse = JsonConvert.DeserializeObject<ChangeTicketTitleRes>(changeTicketTitleResponse.DataAsText)!;
                
                if (!string.IsNullOrEmpty(changeTicketTitleJsonResponse.error_msg))
                {
                    loadingWindow.DestroyWindow();
                    
                    var messageBoxWindow = AppManager.CreateWindow<MessageBoxWindow>(AppManager.Windows.Root);
                    var messageBoxWindowController = new MessageBoxWindowController(AppManager, messageBoxWindow);
                    messageBoxWindowController.Open("Oops!", changeTicketTitleJsonResponse.error_msg, () =>
                    {
                        messageBoxWindow.DestroyWindow();
                    });
                    return;
                }
                
                AppManager.OnUnexpectedApiCallException(this, changeTicketDescriptionRequest, null);
                return;
            }
            
            if (!changeTicketDescriptionResponse.IsSuccess)
            {
                var changeTicketDescriptionJsonResponse = JsonConvert.DeserializeObject<ChangeTicketDescriptionRes>(changeTicketDescriptionResponse.DataAsText)!;
                
                if (!string.IsNullOrEmpty(changeTicketDescriptionJsonResponse.error_msg))
                {
                    loadingWindow.DestroyWindow();
                    
                    var messageBoxWindow = AppManager.CreateWindow<MessageBoxWindow>(AppManager.Windows.Root);
                    var messageBoxWindowController = new MessageBoxWindowController(AppManager, messageBoxWindow);
                    messageBoxWindowController.Open("Oops!", changeTicketDescriptionJsonResponse.error_msg, () =>
                    {
                        messageBoxWindow.DestroyWindow();
                    });
                    return;
                }
                
                AppManager.OnUnexpectedApiCallException(this, changeTicketDescriptionRequest, null);
                return;
            }
            
            loadingWindow.DestroyWindow();
            
            _window.Hide(false, true);
            
            // TODO: do local changes, instead force reopen board window
            AppManager.GetReadyRootWindow<BoardWindow, BoardWindowController>().Reopen();
        }
    }
}