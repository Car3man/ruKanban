namespace RuKanban.App.Window
{
    public class LoadingWindowController : BaseAppWindowController
    {
        private readonly LoadingWindow _window;

        public LoadingWindowController(AppManager appManager, LoadingWindow window)
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }
    }
}