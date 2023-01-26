using TMPro;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class CreateTicketWindow : BaseAppWindow
    {
        public TMP_InputField titleInput;
        public TMP_InputField descriptionInput;
        public Button closeButton;
        public Button createButton;

        protected override void HideWindow(bool force)
        {
            ResetElements();
            base.HideWindow(force);
        }
        
        public override void ResetElements()
        {
            titleInput.text = string.Empty;
            descriptionInput.text = string.Empty;
            closeButton.onClick.RemoveAllListeners();
            createButton.onClick.RemoveAllListeners();
        }
    }
}