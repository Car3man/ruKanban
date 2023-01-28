using UnityEngine;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class BoardWindowHeader  : BaseHeader
    {
        public Button backButton;
        public GameObject syncSpinner;
        
        public override void ResetElements()
        {
            backButton.onClick.RemoveAllListeners();
            syncSpinner.SetActive(false);
            base.ResetElements();
        }
    }
}