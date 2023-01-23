using UnityEngine;
using Zenject;

namespace RuKanban.App
{
    public class AppSceneInstaller : MonoInstaller
    {
        [SerializeField] private Windows windows;

        public override void InstallBindings()
        {
            Container.Bind<Windows>().FromInstance(windows);
        }
    }
}