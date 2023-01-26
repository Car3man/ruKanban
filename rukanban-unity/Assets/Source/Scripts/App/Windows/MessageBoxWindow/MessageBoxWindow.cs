using TMPro;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class MessageBoxWindow : BaseAppWindow
    {
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI messageText;
        public Button okayButton;
        
        protected override void HideWindow(bool force)
        {
            ResetElements();
            base.HideWindow(force);
        }

        public override void ResetElements()
        {
            titleText.text = string.Empty;
            messageText.text = string.Empty;
            okayButton.onClick.RemoveAllListeners();
        }
    }
}