using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class LoginPanel : MonoBehaviour
    {
        public TMP_InputField loginInput;
        public TMP_InputField passwordInput;
        public Button signUpButton;
        public Button signInButton;

        public void ResetElements()
        {
            loginInput.text = string.Empty;
            passwordInput.text = string.Empty;
            
            loginInput.onValueChanged.RemoveAllListeners();
            passwordInput.onValueChanged.RemoveAllListeners();
            signUpButton.onClick.RemoveAllListeners();
            signInButton.onClick.RemoveAllListeners();
        }
    }
}