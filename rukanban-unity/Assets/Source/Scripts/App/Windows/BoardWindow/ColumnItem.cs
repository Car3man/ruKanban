using System;
using System.Collections.Generic;
using DG.Tweening;
using RuKanban.Services.Api.DatabaseModels;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class ColumnItem : MonoBehaviour
    {
        public TextMeshProUGUI titleText;
        public Button titleButton;
        public TMP_InputField editTitleText;
        public Button deleteButton;
        public Button addTicketButton;
        
        [SerializeField] private TicketItem itemTemplate;
        [SerializeField] private Transform itemParent;
        [SerializeField] private TicketItemPlaceholder itemPlaceholder;
        public float Height => GetComponent<RectTransform>().rect.height;
        public Transform TicketItemsParent => itemParent;

        public List<TicketItem> currTicketItems;

        public delegate void TicketItemReadyDelegate(TicketItem ticketItem, Ticket ticket, bool isLocal);
        public delegate void TicketItemClickDelegate(TicketItem ticketItem);
        public delegate void TicketItemBeginDragDelegate(TicketItem ticketItem);
        public delegate void TicketItemDragDelegate(TicketItem ticketItem);
        public delegate void TicketItemEndDragDelegate(TicketItem ticketItem);
        
        public Action<ColumnItem> OnDrag;
        public Action<ColumnItem> OnBeginDrag;
        public Action<ColumnItem> OnEndDrag;
        public TicketItemReadyDelegate OnTicketItemReady;
        public TicketItemClickDelegate OnTicketItemClick;
        public TicketItemBeginDragDelegate OnTicketItemDrag;
        public TicketItemDragDelegate OnTicketItemBeginDrag;
        public TicketItemEndDragDelegate OnTicketItemEndDrag;

        public void ResetElements()
        {
            titleText.text = string.Empty;
            titleButton.onClick.RemoveAllListeners();
            editTitleText.gameObject.SetActive(false);
            editTitleText.onEndEdit.RemoveAllListeners();
            deleteButton.onClick.RemoveAllListeners();
            addTicketButton.onClick.RemoveAllListeners();
            
            itemTemplate.gameObject.SetActive(false);
            itemPlaceholder.gameObject.SetActive(false);
            SetTickets(new List<Ticket>());

            OnDrag = null;
            OnBeginDrag = null;
            OnEndDrag = null;
            OnTicketItemReady = null;
            OnTicketItemClick = null;
            OnTicketItemDrag = null;
            OnTicketItemBeginDrag = null;
            OnTicketItemEndDrag = null;
        }

        public void SetTickets(List<Ticket> tickets)
        {
            if (currTicketItems != null)
            {
                foreach (TicketItem item in currTicketItems)
                {
                    Destroy(item.gameObject);
                }
            }

            currTicketItems = new List<TicketItem>();
            
            foreach (Ticket ticket in tickets)
            {
                TicketItem ticketItem = CreateTicket(ticket);
                OnTicketItemReady?.Invoke(ticketItem, ticket, false);
            }
        }

        public void CreateTicketLocal(string title, string description)
        {
            Ticket ticket = new Ticket
            {
                title = title,
                description = description
            };
            TicketItem ticketItem = CreateTicket(ticket);
            OnTicketItemReady?.Invoke(ticketItem, ticket, true);
        }

        private TicketItem CreateTicket(Ticket ticket)
        {
            TicketItem ticketItem = Instantiate(itemTemplate, itemParent);
            ticketItem.gameObject.SetActive(true);

            ticketItem.titleText.text = ticket.title;
            ticketItem.OnClickEvent = item => OnTicketItemClick?.Invoke(item);
            ticketItem.OnDragEvent = item => OnTicketItemDrag?.Invoke(item);
            ticketItem.OnBeginDragEvent = item => OnTicketItemBeginDrag?.Invoke(item);
            ticketItem.OnEndDragEvent = item => OnTicketItemEndDrag?.Invoke(item);

            currTicketItems.Add(ticketItem);

            return ticketItem;
        }

        public int GetTicketPlaceholderSiblingIndex()
        {
            return itemPlaceholder.transform.GetSiblingIndex();
        }
        
        public void SetTicketPlaceholderActive(bool value, int index = 0, float height = 35)
        {
            itemPlaceholder.gameObject.SetActive(value);
            itemPlaceholder.transform.SetSiblingIndex(index);
            itemPlaceholder.SetHeight(height);
        }

        public void AddTicket(TicketItem ticketItem, TicketItem standAfterItem)
        {
            int siblingIndex = 0;
            if (standAfterItem)
            {
                int standAfterItemSiblingIndex = standAfterItem.transform.GetSiblingIndex();
                siblingIndex = standAfterItemSiblingIndex + 1;
            }
            
            ticketItem.transform.SetParent(itemParent);
            ticketItem.transform.SetSiblingIndex(siblingIndex);
            
            ticketItem.OnClickEvent = item => OnTicketItemClick?.Invoke(item);
            ticketItem.OnDragEvent = item => OnTicketItemDrag?.Invoke(item);
            ticketItem.OnBeginDragEvent = item => OnTicketItemBeginDrag?.Invoke(item);
            ticketItem.OnEndDragEvent = item => OnTicketItemEndDrag?.Invoke(item);

            int standAfterIndex = currTicketItems.IndexOf(standAfterItem) + 1;
            currTicketItems.Insert(standAfterIndex, ticketItem);
        }

        public void RemoveTicket(TicketItem ticketItem)
        {
            ticketItem.transform.SetParent(null);
            currTicketItems.Remove(ticketItem);
        }
        
        public void HandleOnDrag(BaseEventData eventData)
        {
            OnDrag?.Invoke(this);
        }

        public void HandleBeginDrag(BaseEventData eventData)
        {
            transform
                .DOLocalRotate(Vector3.forward * 5f, 0.2f);

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