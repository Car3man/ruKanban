using UnityEngine;

namespace RuKanban.App.Window
{
    public class LoadingWindow : BaseAppWindow
    {
        protected override void ShowWindow(bool force = false)
        {
            base.ShowWindow(force);
            
            GetComponent<RectTransform>().anchorMin = Vector2.zero;
            GetComponent<RectTransform>().anchorMax = Vector2.one;
            GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0f);
            GetComponent<RectTransform>().offsetMax = new Vector2(1f, 1f);
        }

        public override void ResetElements()
        {
            
        }
    }
}