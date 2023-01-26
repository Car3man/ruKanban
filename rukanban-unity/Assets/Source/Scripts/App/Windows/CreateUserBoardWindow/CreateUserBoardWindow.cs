using TMPro;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class CreateUserBoardWindow : BaseAppWindow
    {
        public TMP_InputField nameInput;
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
            nameInput.text = string.Empty;
            descriptionInput.text = string.Empty;
            closeButton.onClick.RemoveAllListeners();
            createButton.onClick.RemoveAllListeners();
        }
    }
}