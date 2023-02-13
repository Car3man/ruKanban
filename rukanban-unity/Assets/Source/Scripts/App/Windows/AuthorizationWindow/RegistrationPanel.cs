using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class RegistrationPanel : MonoBehaviour
    {
        public TMP_InputField loginInput;
        public TMP_InputField passwordInput;
        public TMP_InputField passwordAgainInput;
        public TMP_InputField firstNameInput;
        public TMP_InputField lastNameInput;
        public Button signInButton;
        public Button signUpButton;

        public void ResetElements()
        {
            loginInput.text = string.Empty;
            passwordInput.text = string.Empty;
            passwordAgainInput.text = string.Empty;
            firstNameInput.text = string.Empty;
            lastNameInput.text = string.Empty;
            
            loginInput.onValueChanged.RemoveAllListeners();
            passwordInput.onValueChanged.RemoveAllListeners();
            passwordAgainInput.onValueChanged.RemoveAllListeners();
            firstNameInput.onValueChanged.RemoveAllListeners();
            lastNameInput.onValueChanged.RemoveAllListeners();
            signInButton.onClick.RemoveAllListeners();
            signUpButton.onClick.RemoveAllListeners();
        }
    }
}