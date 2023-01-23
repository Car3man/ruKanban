using RuKanban.Services.AppConfiguration;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace RuKanban
{
    public class EntryPointStarter : MonoBehaviour
    {
        [Inject] private AppConfigurationService _appConfigurationService;

        private void Start()
        {
            TrySetupDesktopWindow();

            SceneManager.LoadScene("App", LoadSceneMode.Single);
        }

        private void TrySetupDesktopWindow()
        {
            if (_appConfigurationService.IsMobileVersion)
            {
                return;
            }
        
            int screenWidth = _appConfigurationService.DesktopWindow.Width;
            int screenHeight = _appConfigurationService.DesktopWindow.Height;
            Screen.SetResolution(screenWidth, screenHeight, FullScreenMode.Windowed);
        }
    }
}