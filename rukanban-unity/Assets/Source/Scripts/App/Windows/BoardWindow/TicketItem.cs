using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class TicketItem : MonoBehaviour
    {
        public Button itemButton;
        public TextMeshProUGUI titleText;
        public RectTransform overlapTrigger;

        public Action<TicketItem> OnDrag;
        public Action<TicketItem> OnBeginDrag;
        public Action<TicketItem> OnEndDrag;

        public bool IsDragging { get; private set; }

        public void HandleOnDrag(BaseEventData eventData)
        {
            OnDrag?.Invoke(this);
        }

        public void HandleBeginDrag(BaseEventData eventData)
        {
            IsDragging = true;
            OnBeginDrag?.Invoke(this);
        }

        public void HandleEndDrag(BaseEventData eventData)
        {
            OnEndDrag?.Invoke(this);
            IsDragging = false;
        }
    }
}