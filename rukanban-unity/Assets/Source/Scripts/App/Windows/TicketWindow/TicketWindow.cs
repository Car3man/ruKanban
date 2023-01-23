using TMPro;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class TicketWindow : BaseAppWindow
    {
        public Button closeButton;
        public TMP_InputField titleInput;
        public TMP_InputField descriptionInput;
        public Button deleteButton;
        public Button saveButton;

        protected override void HideWindow(bool force = false)
        {
            ResetElements();
            
            base.HideWindow(force);
        }

        public override void ResetElements()
        {
            closeButton.onClick.RemoveAllListeners();
            titleInput.text = string.Empty;
            descriptionInput.text = string.Empty;
            deleteButton.onClick.RemoveAllListeners();
            saveButton.onClick.RemoveAllListeners();
        }
    }
}