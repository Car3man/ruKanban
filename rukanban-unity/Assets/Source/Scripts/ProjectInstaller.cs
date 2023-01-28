using RuKanban.Services.Api;
using RuKanban.Services.AppConfiguration;
using RuKanban.Services.Authorization;
using RuKanban.Services.Theme;
using Zenject;

namespace RuKanban
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallServices();
        }

        private void InstallServices()
        {
            Container.Bind<AppConfigurationService>().AsSingle().NonLazy();
            Container.Bind<ThemeService>().AsSingle().NonLazy();
            Container.Bind<ApiService>().AsSingle().NonLazy();
            Container.Bind<AuthorizationService>().AsSingle().NonLazy();
        }
    }
}