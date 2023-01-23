using UnityEngine;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class BaseHeader : MonoBehaviour
    {
        public Button settingsButton;

        public virtual void ResetElements()
        {
            settingsButton.onClick.RemoveAllListeners();
        }
    }
}