using System;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api;
using RuKanban.Services.Api.Exceptions;
using RuKanban.Services.Api.Response.Ticket;

namespace RuKanban.App.Window
{
    public class CreateTicketWindowController : BaseAppWindowController
    {
        private readonly CreateTicketWindow _window;
        private string _columnId;

        public CreateTicketWindowController(AppManager appManager, CreateTicketWindow window)
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }

        public void Open(string columnId)
        {
            if (_window.IsActive())
            {
                _window.Hide(true, true);
            }
            
            _columnId = columnId;
            
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

            string ticketTitle = _window.titleInput.text;
            string ticketDescription = _window.descriptionInput.text;
            ApiRequest createTicketRequest = AppManager.ApiService.Ticket.CreateTicket(_columnId, ticketTitle, ticketDescription);
            HTTPResponse createTicketResponse;
            
            try { createTicketResponse = await AppManager.AuthorizedApiCall(this, createTicketRequest); }
            catch (Exception exception) when (exception is not UnauthorizedApiRequest)
            {
                AppManager.OnUnexpectedApiCallException(this, createTicketRequest, exception);
                return;
            }
            
            var createTicketJsonResponse = JsonConvert.DeserializeObject<CreateTicketRes>(createTicketResponse.DataAsText)!;
            if (!createTicketResponse.IsSuccess)
            {
                if (!string.IsNullOrEmpty(createTicketJsonResponse.error_msg))
                {
                    loadingWindow.DestroyWindow();
                    
                    var messageBoxWindow = AppManager.CreateWindow<MessageBoxWindow>(_window);
                    var messageBoxWindowController = new MessageBoxWindowController(AppManager, messageBoxWindow);
                    messageBoxWindowController.Open("Oops!", createTicketJsonResponse.error_msg, () =>
                    {
                        messageBoxWindow.DestroyWindow();
                    });
                    return;
                }
                
                AppManager.OnUnexpectedApiCallException(this, createTicketRequest, null);
                return;
            }
            
            loadingWindow.DestroyWindow();
            
            _window.Hide(false, true);
            AppManager.GetReadyRootWindow<BoardWindow, BoardWindowController>().Reopen();
        }
    }
}