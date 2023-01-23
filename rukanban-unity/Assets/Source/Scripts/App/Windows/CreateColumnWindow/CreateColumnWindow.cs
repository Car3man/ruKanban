using TMPro;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class CreateColumnWindow : BaseAppWindow
    {
        public TMP_InputField nameInput;
        public Button closeButton;
        public Button createButton;

        protected override void HideWindow(bool force = false)
        {
            ResetElements();
            base.HideWindow(force);
        }
        
        public override void ResetElements()
        {
            nameInput.text = string.Empty;
            closeButton.onClick.RemoveAllListeners();
            createButton.onClick.RemoveAllListeners();
        }
    }
}