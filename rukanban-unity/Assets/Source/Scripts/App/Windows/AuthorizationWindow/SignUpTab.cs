using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class SignUpTab : MonoBehaviour
    {
        public TMP_InputField loginInput;
        public TMP_InputField passwordInput;
        public TMP_InputField firstNameInput;
        public TMP_InputField surNameInput;
        public TMP_InputField patronymicInput;
        public Button signInButton;
        public Button signUpButton;

        public void ResetElements()
        {
            loginInput.text = string.Empty;
            passwordInput.text = string.Empty;
            firstNameInput.text = string.Empty;
            surNameInput.text = string.Empty;
            patronymicInput.text = string.Empty;
            
            loginInput.onValueChanged.RemoveAllListeners();
            passwordInput.onValueChanged.RemoveAllListeners();
            firstNameInput.onValueChanged.RemoveAllListeners();
            surNameInput.onValueChanged.RemoveAllListeners();
            patronymicInput.onValueChanged.RemoveAllListeners();
            signInButton.onClick.RemoveAllListeners();
            signUpButton.onClick.RemoveAllListeners();
        }
    }
}