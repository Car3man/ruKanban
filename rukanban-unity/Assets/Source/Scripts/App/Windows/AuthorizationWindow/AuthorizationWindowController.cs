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
        
        private SignInTab SignInTab => _window.signInTab;
        private SignUpTab SignUpTab => _window.signUpTab;
        
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
            
            SignInTab.gameObject.SetActive(false);
            SignUpTab.gameObject.SetActive(false);
            
            ShowSignInWindow();
        }

        private void ShowSignInWindow()
        {
            if (SignUpTab.gameObject.activeSelf)
            {
                SignUpTab.ResetElements();
                SignUpTab.gameObject.SetActive(false);
            }

            if (SignInTab.gameObject.activeSelf)
            {
                return;
            }
            
            SignInTab.signUpButton.onClick.AddListener(SignInWindow_OnSignUpButtonClick);
            SignInTab.signInButton.onClick.AddListener(SignInWindow_OnSignInButtonClick);
            SignInTab.gameObject.SetActive(true);
        }

        private void SignInWindow_OnSignUpButtonClick()
        {
            ShowSignUpWindow();
        }

        private async void SignInWindow_OnSignInButtonClick()
        {
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);
   
            string login = SignInTab.loginInput.text;
            string password = SignInTab.passwordInput.text;
            
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
            if (SignInTab.gameObject.activeSelf)
            {
                SignInTab.ResetElements();
                SignInTab.gameObject.SetActive(false);
            }
            
            if (SignUpTab.gameObject.activeSelf)
            {
                return;
            }
            
            SignUpTab.signInButton.onClick.AddListener(SignUpWindow_OnSignInButtonClick);
            SignUpTab.signUpButton.onClick.AddListener(SignUpWindow_OnSignUpButtonClick);
            SignUpTab.gameObject.SetActive(true);
        }
        
        private void SignUpWindow_OnSignInButtonClick()
        {
            ShowSignInWindow();
        }

        private async void SignUpWindow_OnSignUpButtonClick()
        {
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);
   
            string login = SignUpTab.loginInput.text;
            string password = SignUpTab.passwordInput.text;
            string firstName = SignUpTab.firstNameInput.text;
            string surName = SignUpTab.surNameInput.text;
            string patronymic = SignUpTab.patronymicInput.text;
            
            ApiRequest signUpRequest = AppManager.ApiService.Auth.SignUp(login, password, firstName, surName, patronymic);
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