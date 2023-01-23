using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class UserBoardsWindowHeader : BaseHeader
    {
        public Button backButton;
        
        public override void ResetElements()
        {
            backButton.onClick.RemoveAllListeners();
            base.ResetElements();
        }
    }
}