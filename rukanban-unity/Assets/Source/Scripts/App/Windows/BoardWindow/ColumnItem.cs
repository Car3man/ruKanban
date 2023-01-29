using System.Collections.Generic;
using RuKanban.Services.Api.DatabaseModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class ColumnItem : MonoBehaviour
    {
        public TextMeshProUGUI titleText;
        public Button deleteButton;
        public Button addTicketButton;
        
        [SerializeField] private TicketItem itemTemplate;
        [SerializeField] private Transform itemParent;
        [SerializeField] private TicketItemPlaceholder itemPlaceholder;
        public Transform TicketItemsParent => itemParent;
        
        public List<TicketItem> currTicketItems;

        public delegate void TicketItemReadyDelegate(TicketItem ticketItem, Ticket ticket, bool isLocal);
        public delegate void TicketItemClickDelegate(TicketItem ticketItem);
        public delegate void TicketItemBeginDragDelegate(TicketItem ticketItem);
        public delegate void TicketItemDragDelegate(TicketItem ticketItem);
        public delegate void TicketItemEndDragDelegate(TicketItem ticketItem);
        
        public TicketItemReadyDelegate OnTicketItemReady;
        public TicketItemClickDelegate OnTicketItemClick;
        public TicketItemBeginDragDelegate OnTicketItemDrag;
        public TicketItemDragDelegate OnTicketItemBeginDrag;
        public TicketItemEndDragDelegate OnTicketItemEndDrag;

        public void ResetElements()
        {
            titleText.text = string.Empty;
            deleteButton.onClick.RemoveAllListeners();
            addTicketButton.onClick.RemoveAllListeners();
            
            itemTemplate.gameObject.SetActive(false);
            itemPlaceholder.gameObject.SetActive(false);
            SetTickets(new List<Ticket>());
            
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
            ticketItem.OnClick = item => OnTicketItemClick?.Invoke(item);
            ticketItem.OnDrag = item => OnTicketItemDrag?.Invoke(item);
            ticketItem.OnBeginDrag = item => OnTicketItemBeginDrag?.Invoke(item);
            ticketItem.OnEndDrag = item => OnTicketItemEndDrag?.Invoke(item);

            currTicketItems.Add(ticketItem);

            return ticketItem;
        }

        public void SetTicketPlaceholderActive(bool value, int index = 0, float height = 35)
        {
            itemPlaceholder.gameObject.SetActive(value);
            itemPlaceholder.transform.SetSiblingIndex(index);
            itemPlaceholder.SetHeight(height);
        }

        public void AddTicket(TicketItem ticketItem, TicketItem standAfterItem)
        {
            int siblingIndex = standAfterItem != null ? standAfterItem.transform.GetSiblingIndex() + 1 : 0;
            
            ticketItem.transform.SetParent(itemParent);
            ticketItem.transform.SetSiblingIndex(siblingIndex);
            
            ticketItem.OnClick = item => OnTicketItemClick?.Invoke(item);
            ticketItem.OnDrag = item => OnTicketItemDrag?.Invoke(item);
            ticketItem.OnBeginDrag = item => OnTicketItemBeginDrag?.Invoke(item);
            ticketItem.OnEndDrag = item => OnTicketItemEndDrag?.Invoke(item);

            int standAfterIndex = currTicketItems.IndexOf(standAfterItem) + 1;
            currTicketItems.Insert(standAfterIndex, ticketItem);
        }

        public void RemoveTicket(TicketItem ticketItem)
        {
            ticketItem.transform.SetParent(null);
            currTicketItems.Remove(ticketItem);
        }
    }
}