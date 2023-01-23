using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RuKanban.Window
{
    public class BaseWindow : MonoBehaviour
    {
        internal bool DestroyOnHide;
        
        [HideInInspector] public bool runtimeCreated;
        
        private readonly List<BaseWindow> _children = new ();
        private BaseWindowController _controller;

        public void BindController(BaseWindowController controller)
        {
            _controller = controller;
        }

        public BaseWindowController GetController()
        {
            return _controller;
        }
        
        public List<BaseWindow> GetChildren()
        {
            return new List<BaseWindow>(_children);
        }
        
        public T GetChildWindow<T>() where T : BaseWindow
        {
            BaseWindow child = _children.FirstOrDefault(x => x.GetType() == typeof(T));
            if (child == null)
            {
                throw new Exception($"{GetType()} window doesn't contains next child window: {typeof(T)}");
            }
            
            return (T)child;
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }
        
        public void Show(bool force = false)
        {
            if (IsActive())
            {
                Debug.LogWarning($"You trying open opened window: {GetType()}", this);
            }
            
            ShowWindow(force);
            _controller?.OnWindowShow(this);
        }

        public void Hide(bool force = false, bool recursive = true)
        {
            if (recursive)
            {
                foreach (BaseWindow childWindow in _children)
                {
                    if (childWindow == null)
                    {
                        continue;
                    }
                    
                    childWindow.Hide(force, true);
                }
            }

            HideWindow(force);
            _controller?.OnWindowHide(this);

            if (DestroyOnHide)
            {
                DestroyWindow();
            }
        }

        protected virtual void ShowWindow(bool force = false)
        {
            gameObject.SetActive(true);
        }

        protected virtual void HideWindow(bool force = false)
        {
            gameObject.SetActive(false);
        }
        
        public void DestroyWindow()
        {
            foreach (BaseWindow childWindow in _children)
            {
                if (childWindow == null)
                {
                    continue;
                }
                
                childWindow.DestroyWindow();
            }
            
            _children.Clear();

            Destroy(gameObject);
            _controller?.OnWindowDestroy(this);
        }
        
        internal void AddChild(BaseWindow childWindow)
        {
            _children.Add(childWindow);
        }
        
        internal void SetChildren(List<BaseWindow> childrenWindow)
        {
            _children.Clear();
            _children.AddRange(childrenWindow);
        }

        internal void ClearChildren()
        {
            _children.Clear();
        }
    }
}