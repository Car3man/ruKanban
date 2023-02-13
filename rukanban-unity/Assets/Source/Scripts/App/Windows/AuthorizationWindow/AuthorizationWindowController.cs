using System;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.Services.Api;
using RuKanban.Services.Api.Response.Auth;
using RuKanban.Services.Authorization;

namespace RuKanban.App.Window
{
    public class AuthorizationWindowController : BaseAppWindowController
    {
        private readonly AuthorizationWindow _window;
        
        private LoginPanel LoginPanel => _window.loginPanel;
        private RegistrationPanel RegistrationPanel => _window.registrationPanel;
        
        public AuthorizationWindowController(AppManager appManager, AuthorizationWindow window) 
            : base(appManager, window)
        {
            _window = window;
            _window.BindController(this);
        }

        public void Open()
        {
            if (_window.IsActive())
            {
                _window.Hide(true, true);
            }
            
            _window.Show(false);
            
            LoginPanel.gameObject.SetActive(false);
            RegistrationPanel.gameObject.SetActive(false);
            
            ShowSignInWindow();
        }

        private void ShowSignInWindow()
        {
            if (RegistrationPanel.gameObject.activeSelf)
            {
                RegistrationPanel.ResetElements();
                RegistrationPanel.gameObject.SetActive(false);
            }

            if (LoginPanel.gameObject.activeSelf)
            {
                return;
            }
            
            LoginPanel.signUpButton.onClick.AddListener(SignInWindow_OnSignUpButtonClick);
            LoginPanel.signInButton.onClick.AddListener(SignInWindow_OnSignInButtonClick);
            LoginPanel.gameObject.SetActive(true);
        }

        private void SignInWindow_OnSignUpButtonClick()
        {
            ShowSignUpWindow();
        }

        private async void SignInWindow_OnSignInButtonClick()
        {
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);
   
            string login = LoginPanel.loginInput.text;
            string password = LoginPanel.passwordInput.text;
            
            ApiRequest signInRequest = AppManager.ApiService.Auth.SignIn(login, password);
            HTTPResponse signInResponse;

            try { signInResponse = await signInRequest.GetHTTPResponseAsync(); }
            catch (Exception exception)
            {
                AppManager.OnUnexpectedApiCallException(this, signInRequest, exception);
                return;
            }

            var signInJsonResponse = JsonConvert.DeserializeObject<SignInRes>(signInResponse.DataAsText)!;
            if (!signInResponse.IsSuccess)
            {
                if (!string.IsNullOrEmpty(signInJsonResponse.error_msg))
                {
                    loadingWindow.DestroyWindow();
                    
                    var messageBoxWindow = AppManager.Windows.Create<MessageBoxWindow>(_window);
                    var messageBoxWindowController = new MessageBoxWindowController(AppManager, messageBoxWindow);
                    messageBoxWindowController.Open("Oops!", signInJsonResponse.error_msg, () =>
                    {
                        messageBoxWindow.DestroyWindow();
                    });
                    return;
                }
                
                AppManager.OnUnexpectedApiCallException(this, signInRequest, null);
                return;
            }
            
            loadingWindow.DestroyWindow();

            AppManager.AuthorizationService.AuthorizationData = new AuthorizationData(
                signInJsonResponse!.access_token,
                signInJsonResponse!.refresh_token,
                signInJsonResponse!.user_id,
                signInJsonResponse!.login
            );
            
            GoToMainAppWindow();
        }
        
        private void ShowSignUpWindow()
        {
            if (LoginPanel.gameObject.activeSelf)
            {
                LoginPanel.ResetElements();
                LoginPanel.gameObject.SetActive(false);
            }
            
            if (RegistrationPanel.gameObject.activeSelf)
            {
                return;
            }
            
            RegistrationPanel.signInButton.onClick.AddListener(SignUpWindow_OnSignInButtonClick);
            RegistrationPanel.signUpButton.onClick.AddListener(SignUpWindow_OnSignUpButtonClick);
            RegistrationPanel.gameObject.SetActive(true);
        }
        
        private void SignUpWindow_OnSignInButtonClick()
        {
            ShowSignInWindow();
        }

        private async void SignUpWindow_OnSignUpButtonClick()
        {
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);
   
            string login = RegistrationPanel.loginInput.text;
            string password = RegistrationPanel.passwordInput.text;
            string passwordAgain = RegistrationPanel.passwordAgainInput.text;
            string firstName = RegistrationPanel.firstNameInput.text;
            string lastNameInput = RegistrationPanel.lastNameInput.text;
            
            // TODO: check if password equals to password again
            
            ApiRequest signUpRequest = AppManager.ApiService.Auth.SignUp(login, password, firstName, lastNameInput);
            HTTPResponse signUpResponse;
            
            try { signUpResponse = await signUpRequest.GetHTTPResponseAsync(); }
            catch (Exception exception)
            {
                AppManager.OnUnexpectedApiCallException(this, signUpRequest, exception);
                return;
            }

            var signUpJsonResponse = JsonConvert.DeserializeObject<SignUpRes>(signUpResponse.DataAsText)!;
            if (!signUpResponse.IsSuccess)
            {
                if (!string.IsNullOrEmpty(signUpJsonResponse.error_msg))
                {
                    loadingWindow.DestroyWindow();
                    
                    var messageBoxWindow = AppManager.CreateWindow<MessageBoxWindow>(_window);
                    var messageBoxWindowController = new MessageBoxWindowController(AppManager, messageBoxWindow);
                    messageBoxWindowController.Open("Oops!", signUpJsonResponse.error_msg, () =>
                    {
                        messageBoxWindow.DestroyWindow();
                    });
                    return;
                }
                
                AppManager.OnUnexpectedApiCallException(this, signUpRequest, null);
                return;
            }
            
            loadingWindow.DestroyWindow();

            AppManager.AuthorizationService.AuthorizationData = new AuthorizationData(
                signUpJsonResponse!.access_token,
                signUpJsonResponse!.refresh_token,
                signUpJsonResponse!.user_id,
                signUpJsonResponse!.login
            );

            GoToMainAppWindow();
        }

        private void GoToMainAppWindow()
        {
            _window.Hide(false, true);
            
            AppManager.GetReadyRootWindow<UserWorkspacesWindow, UserWorkspacesWindowController>().Open();
        }
    }
}