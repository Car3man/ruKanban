using System;
using RuKanban.Window;
using UnityEngine;

namespace RuKanban
{
    public class Windows : MonoBehaviour
    {
        private string _loadFromBasePath;
        private Canvas _windowsCanvas;
        
        public BaseWindow Root { get; private set; }

        public void Initialize(string loadFromBasePath)
        {
            _loadFromBasePath = loadFromBasePath;
            
            FixRootWindow();
            FindWindowChildren(Root);
            Root.Hide(true, true);
        }

        private void FixRootWindow()
        {
            _windowsCanvas = GetComponentInChildren<Canvas>();
            if (_windowsCanvas == null)
            {
                throw new Exception("Windows gameObject should contains Canvas");
            }

            Transform windowsCanvasRoot = _windowsCanvas.transform;

            if (windowsCanvasRoot.childCount > 1)
            {
                throw new Exception("Windows Canvas gameObject should contains only one root window gameObject");
            }
            
            if (windowsCanvasRoot.childCount == 1)
            {
                Transform child = windowsCanvasRoot.GetChild(0);
                Root = child.GetComponent<BaseWindow>();

                if (Root == null)
                {
                    throw new Exception("Windows Canvas gameObject should contains root window (BaseWindow)");
                }
            }
            else
            {
                GameObject rootObject = new GameObject("RootWindow");
                rootObject.transform.SetParent(windowsCanvasRoot);
                Root = rootObject.AddComponent<BaseWindow>();
            }
        }

        private void FindWindowChildren(BaseWindow root)
        {
            root.ClearChildren();
            
            foreach (Transform childTransform in root.transform)
            {
                var child = childTransform.GetComponent<BaseWindow>();
                if (child == null)
                {
                    continue;
                }
                
                FindWindowChildren(child);
                root.AddChild(child);
            }
        }

        public T Create<T>(BaseWindow parent, bool destroyOnHide = true) where T : BaseWindow
        {
            GameObject windowPrefab = GetWindowPrefab<T>();
            GameObject windowObject = Instantiate(windowPrefab, parent.transform);
            BaseWindow windowInstance = windowObject.GetComponent<BaseWindow>();
            windowInstance.runtimeCreated = true;
            windowInstance.DestroyOnHide = destroyOnHide;
            FindWindowChildren(windowInstance);
            parent.AddChild(windowInstance);
            return (T)windowInstance;
        }
        
        public T CreateAndShow<T>(BaseWindow parent, bool destroyOnHide = true, bool force = false) where T : BaseWindow
        {
            T baseWindow = Create<T>(parent, destroyOnHide);
            baseWindow.Show(force);
            return baseWindow;
        }

        private GameObject GetWindowPrefab<T>()
        {
            return Resources.Load<GameObject>($"{_loadFromBasePath}/{typeof(T).Name}");
        }
    }
}