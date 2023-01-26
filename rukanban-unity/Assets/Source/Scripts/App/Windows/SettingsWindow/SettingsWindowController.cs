using System;
using RuKanban.Services.Api;
using RuKanban.Services.Authorization;

namespace RuKanban.App.Window
{
    public class SettingsWindowController : BaseAppWindowController
    {
        private readonly SettingsWindow _window;

        public SettingsWindowController(AppManager appManager, SettingsWindow window) 
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
            
            _window.closeButton.onClick.AddListener(OnCloseButtonClick);
            _window.signOutButton.onClick.AddListener(OnSignOutButtonClick);
            _window.Show(false);
        }

        private void OnCloseButtonClick()
        {
            _window.Hide(false, true);
        }

        private async void OnSignOutButtonClick()
        {
            var loadingWindow = AppManager.CreateAndShowWindow<LoadingWindow, LoadingWindowController>(AppManager.Windows.Root);
            
            ApiRequest signOutRequest = AppManager.ApiService.Auth.SignOut();

            try
            {
                await AppManager.AuthorizedApiCall(this, signOutRequest);
            }
            catch
            {
                // ignored
            }

            loadingWindow.DestroyWindow();

            AppManager.AuthorizationService.AuthorizationData = AuthorizationData.Empty();

            GoToAuthorizationWindow();
        }

        private void GoToAuthorizationWindow()
        {
            AppManager.HideAllWindows(true);
            AppManager.GetReadyRootWindow<AuthorizationWindow, AuthorizationWindowController>().Open();
        }
    }
}