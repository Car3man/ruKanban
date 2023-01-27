namespace RuKanban.App.Window
{
    public class CreateTicketWindowController : BaseAppWindowController
    {
        private readonly CreateTicketWindow _window;
        private CreateTicketCallbackDelegate _callback;
        
        public delegate void CreateTicketCallbackDelegate(string title, string description);

        public CreateTicketWindowController(AppManager appManager, CreateTicketWindow window)
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }

        public void Open(CreateTicketCallbackDelegate callback)
        {
            if (_window.IsActive())
            {
                _window.Hide(true, true);
            }
            
            _callback = callback;
            
            _window.Show(false);
            _window.closeButton.onClick.AddListener(OnCloseButtonClick);
            _window.createButton.onClick.AddListener(OnCreateButtonClick);
        }

        private void OnCloseButtonClick()
        {
            _window.Hide(false, true);
        }

        private void OnCreateButtonClick()
        {
            string title = _window.titleInput.text;
            string description = _window.descriptionInput.text;
            _callback?.Invoke(title, description);
            
            _window.Hide(false, true);
        }
    }
}