using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class SettingsWindow : BaseAppWindow
    {
        public Button closeButton;
        public Button signOutButton;
        
        protected override void HideWindow(bool force)
        {
            ResetElements();
            base.HideWindow(force);
        }

        public override void ResetElements()
        {
            closeButton.onClick.RemoveAllListeners();
            signOutButton.onClick.RemoveAllListeners();
        }
    }
}