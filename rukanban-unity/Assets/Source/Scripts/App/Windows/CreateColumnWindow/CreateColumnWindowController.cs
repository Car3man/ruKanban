namespace RuKanban.App.Window
{
    public class CreateColumnWindowController : BaseAppWindowController
    {
        private readonly CreateColumnWindow _window;
        private CreateColumnCallbackDelegate _callback;
        
        public delegate void CreateColumnCallbackDelegate(string title);

        public CreateColumnWindowController(AppManager appManager, CreateColumnWindow window)
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }

        public void Open(CreateColumnCallbackDelegate callback)
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
            string title = _window.nameInput.text;
            _callback?.Invoke(title);
            
            _window.Hide(false, true);
        }
    }
}