using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BestHTTP;
using Newtonsoft.Json;
using RuKanban.App.Window;
using RuKanban.Services.Api;
using RuKanban.Services.Api.Exceptions;
using RuKanban.Services.Api.Response.Auth;
using RuKanban.Services.AppConfiguration;
using RuKanban.Services.Authorization;
using RuKanban.Services.Theme;
using RuKanban.Window;
using UnityEngine;
using Zenject;

namespace RuKanban.App
{
    public class AppManager : MonoBehaviour
    {
        [Inject] public AppConfigurationService AppConfigurationService { get; private set; }
        [Inject] public ApiService ApiService { get; private set; }
        [Inject] public AuthorizationService AuthorizationService { get; private set; }
        [Inject] public ThemeService ThemeService { get; private set; }
        [Inject] public Windows Windows { get; private set; }

        private readonly HashSet<BaseAppWindowController> _tickableWindowControllers = new();

        private void Start()
        {
            InitializeWindows();
            GetReadyRootWindow<UserWorkspacesWindow, UserWorkspacesWindowController>().Open();
        }

        private void Update()
        {
            _tickableWindowControllers.RemoveWhere(x => x == null);
            
            foreach (BaseAppWindowController windowController in _tickableWindowControllers)
            {
                windowController.Tick();
            }
        }

        private void InitializeWindows()
        {
            bool isPortraitMode = AppConfigurationService.IsMobileVersion;
            #if FORCE_PORTRAIT
            isPortraitMode = true;
            #elif FORCE_LANDSCAPE
            isPortraitMode = false;
            #endif

            Screen.orientation = isPortraitMode ? ScreenOrientation.Portrait : ScreenOrientation.LandscapeLeft;
            
            string windowsBaseResourcePath = isPortraitMode ? "Windows/Portrait" : "Windows/Landscape";
            Instantiate(Resources.Load<GameObject>($"{windowsBaseResourcePath}/Windows"), Windows.transform);
            Windows.Initialize(windowsBaseResourcePath);
            Windows.Root.Show(false);
        }

        public void MakeWindowControllerTickable(BaseAppWindowController windowController)
        {
            _tickableWindowControllers.Add(windowController);
        }

        public void HideAllWindows(bool force = false, bool includeRoot = false)
        {
            if (includeRoot)
            {
                Windows.Root.Hide(force, true);
            }
            else
            {
                foreach (BaseWindow child in Windows.Root.GetChildren())
                {
                    if (child == null)
                    {
                        continue;
                    }
                    
                    child.Hide(force, true);
                }
            }
        }
        
        public T CreateWindow<T>(BaseWindow root) where T : BaseAppWindow
        {
            return Windows.Create<T>(root);
        }
        
        public T CreateAndShowWindow<T>(BaseWindow root, bool force = false) where T : BaseAppWindow
        {
            T window = CreateWindow<T>(root);
            window.Show(force);
            return window;
        }
        
        public T1 CreateAndShowWindow<T1, T2>(BaseWindow root) where T1 : BaseAppWindow where T2 : BaseAppWindowController
        {
            T1 window = CreateWindow<T1>(root);
            GetOrCreateWindowController<T2>(window);
            return window; 
        }
        
        public T2 GetReadyRootWindow<T1, T2>() where T1 : BaseAppWindow where T2 : BaseAppWindowController
        {
            var window = Windows.Root.GetChildWindow<T1>();
            var windowController = GetOrCreateWindowController<T2>(window);
            return windowController;
        }

        private T GetOrCreateWindowController<T>(BaseWindow window) where T : BaseAppWindowController
        {
            if (window.GetController() != null && window.GetController().GetType() == typeof(T))
            {
                return (T)window.GetController();
            }
            return (T)Activator.CreateInstance(typeof(T),  this, window);
        }

        public async Task<HTTPResponse> ApiCall(BaseAppWindowController caller, ApiRequest request, bool authorized = true)
        {
            try
            {
                if (authorized)
                {
                    request = AuthorizationService.GetRequestWithAuthorization(request);
                }
                
                HTTPResponse response = await request.GetHTTPResponseAsync();
                
                if (authorized && response.StatusCode == 401)
                {
                    string accessToken = AuthorizationService.AuthorizationData.AccessToken;
                    string refreshToken = AuthorizationService.AuthorizationData.RefreshToken;
                    
                    if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                    {
                        ApiRequest refreshTokensRequest = ApiService.Auth.RefreshTokens(refreshToken);
                        refreshTokensRequest.AddHeader("Authorization", $"Bearer {accessToken}");
                        HTTPResponse refreshTokensResponse = await refreshTokensRequest.GetHTTPResponseAsync();
                        if (refreshTokensResponse.IsSuccess)
                        {
                            var refreshTokensJsonResponse = JsonConvert.DeserializeObject<RefreshTokensRes>(refreshTokensResponse.DataAsText);
                        
                            AuthorizationService.AuthorizationData = new AuthorizationData(
                                refreshTokensJsonResponse!.access_token,
                                refreshTokensJsonResponse!.refresh_token,
                                AuthorizationService.AuthorizationData.UserId,
                                AuthorizationService.AuthorizationData.Login
                            );
                            
                            return await ApiCall(caller, request);
                        }
                    }

                    Windows.Root.Hide(true, true);
                    Windows.Root.Show(true);
                    var authorizationWindow = Windows.Root.GetChildWindow<AuthorizationWindow>();
                    var authorizationWindowController = new AuthorizationWindowController(this, authorizationWindow);
                    authorizationWindowController.Open();

                    throw new UnauthorizedApiRequest("Unauthorized request detected, returning to authorization window.");
                }
                
                return response;
            }
            catch (Exception exception)
            {
                OnUnexpectedApiCallException(caller, request, exception);
                throw;
            }
        }

        public void OnUnexpectedApiCallException(BaseWindowController caller, ApiRequest request, Exception exception)
        {
            // Unexpected behaviour
            // Show 'Something went wrong'
            // Then start from main app window again

            throw new Exception("Unexpected api request call");
        }
    }
}