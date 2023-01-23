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
            Container.Bind<AppConfigurationService>().AsSingle();
            Container.Bind<ThemeService>().AsSingle();
            Container.Bind<ApiService>().AsSingle();
            Container.Bind<AuthorizationService>().AsSingle();
        }
    }
}