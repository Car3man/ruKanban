using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace RuKanban.App.Window
{
    public class TicketItem : MonoBehaviour
    {
        public TextMeshProUGUI titleText;
        public RectTransform overlapTrigger;
        
        public Action<TicketItem> OnClick;
        public Action<TicketItem> OnDrag;
        public Action<TicketItem> OnBeginDrag;
        public Action<TicketItem> OnEndDrag;
        
        public float Height => GetComponent<RectTransform>().rect.height;

        private bool _pointerDownValid;

        public void HandlePointerDown(BaseEventData eventData)
        {
            _pointerDownValid = true;
        }

        public void HandlePointerUp(BaseEventData eventData)
        {
            if (_pointerDownValid)
            {
                OnClick?.Invoke(this);
            }
        }

        public void HandleOnDrag(BaseEventData eventData)
        {
            OnDrag?.Invoke(this);
        }

        public void HandleBeginDrag(BaseEventData eventData)
        {
            transform
                .DOLocalRotate(Vector3.forward * 5f, 0.2f);

            _pointerDownValid = false;
            OnBeginDrag?.Invoke(this);
        }

        public void HandleEndDrag(BaseEventData eventData)
        {
            transform
                .DOLocalRotate(Vector3.zero, 0.1f);

            OnEndDrag?.Invoke(this);
        }
    }
}