using RuKanban.Window;

namespace RuKanban.App.Window
{
    public abstract class BaseAppWindowController : BaseWindowController
    {
        protected readonly AppManager AppManager;

        public BaseAppWindowController(AppManager appManager, BaseAppWindow baseWindow)
        {
            AppManager = appManager;

            var themeColorImages = baseWindow.GetComponentsInChildren<ThemeColorImage>(true);
            foreach (ThemeColorImage themeColorImage in themeColorImages)
            {
                themeColorImage.Construct(AppManager.ThemeService);
            }
        }
    }
}