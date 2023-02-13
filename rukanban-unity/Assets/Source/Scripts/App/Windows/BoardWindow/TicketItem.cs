using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace RuKanban.App.Window
{
    public class TicketItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public TextMeshProUGUI titleText;
        public RectTransform overlapTrigger;
        
        public Action<TicketItem> OnClickEvent;
        public Action<TicketItem> OnDragEvent;
        public Action<TicketItem> OnBeginDragEvent;
        public Action<TicketItem> OnEndDragEvent;
        
        public float Height => GetComponent<RectTransform>().rect.height;

        private bool _pointerDownValid;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _pointerDownValid = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_pointerDownValid)
            {
                OnClickEvent?.Invoke(this);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragEvent?.Invoke(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform
                .DOLocalRotate(Vector3.forward * 5f, 0.2f);

            _pointerDownValid = false;
            OnBeginDragEvent?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform
                .DOLocalRotate(Vector3.zero, 0.1f);

            OnEndDragEvent?.Invoke(this);
        }
    }
}