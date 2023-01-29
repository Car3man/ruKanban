using UnityEngine;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    [RequireComponent(typeof(LayoutElement))]
    public class ColumnItemPlaceholder : MonoBehaviour
    {
        public void SetHeight(float height)
        {
            GetComponent<LayoutElement>().preferredHeight = height;
        }
    }
}