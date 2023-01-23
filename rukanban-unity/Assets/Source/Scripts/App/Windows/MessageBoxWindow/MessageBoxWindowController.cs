using System;

namespace RuKanban.App.Window
{
    public class MessageBoxWindowController : BaseAppWindowController
    {
        private readonly MessageBoxWindow _window;

        public MessageBoxWindowController(AppManager appManager, MessageBoxWindow window)
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }

        public void Open(string title, string message, Action okayButtonCallback)
        {
            if (!_window.runtimeCreated && _window.IsActive())
            {
                _window.Hide(true);
            }
            
            _window.titleText.text = title;
            _window.messageText.text = message;
            _window.okayButton.onClick.AddListener(() => okayButtonCallback?.Invoke());
            _window.Show();
        }
    }
}