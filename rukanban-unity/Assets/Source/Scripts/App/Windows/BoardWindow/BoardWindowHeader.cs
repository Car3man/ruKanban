using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class BoardWindowHeader  : BaseHeader
    {
        public Button backButton;
        
        public override void ResetElements()
        {
            backButton.onClick.RemoveAllListeners();
            base.ResetElements();
        }
    }
}