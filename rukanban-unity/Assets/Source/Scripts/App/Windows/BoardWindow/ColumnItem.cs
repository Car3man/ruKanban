using System;
using System.Collections.Generic;
using RuKanban.Services.Api.DatabaseModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class ColumnItem : MonoBehaviour
    {
        public TextMeshProUGUI nameText;
        public Button deleteButton;
        public Button addTicketButton;
        
        [SerializeField] private TicketItem itemTemplate;
        [SerializeField] private Transform itemParent;
        [SerializeField] private TicketItemPlaceholder itemPlaceholder;
        public Transform TicketItemsParent => itemParent;
        
        public List<Ticket> currTickets;
        public List<TicketItem> currTicketItems;

        public Action<Ticket> OnTicketItemClick;
        public Action<Ticket, TicketItem> OnTicketItemDrag;
        public Action<Ticket, TicketItem> OnTicketItemBeginDrag;
        public Action<Ticket, TicketItem> OnTicketItemEndDrag;

        public void ResetElements()
        {
            nameText.text = string.Empty;
            deleteButton.onClick.RemoveAllListeners();
            addTicketButton.onClick.RemoveAllListeners();
            itemTemplate.gameObject.SetActive(false);
            SetTickets(new List<Ticket>());
            OnTicketItemClick = null;
            OnTicketItemDrag = null;
            OnTicketItemBeginDrag = null;
            OnTicketItemEndDrag = null;
            itemPlaceholder.gameObject.SetActive(false);
        }
        
        public void SetTickets(List<Ticket> tickets)
        {
            currTickets = tickets;
            
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
                TicketItem ticketItem = Instantiate(itemTemplate, itemParent);
                ticketItem.gameObject.SetActive(true);

                ticketItem.currTicket = ticket;
                ticketItem.itemButton.onClick.AddListener(() =>
                {
                    if (ticketItem.IsDragging)
                    {
                        return;
                    }
                    
                    OnTicketItemClick?.Invoke(ticket);
                });
                ticketItem.OnDrag = item => OnTicketItemDrag?.Invoke(ticket, item);
                ticketItem.OnBeginDrag = item => OnTicketItemBeginDrag?.Invoke(ticket, item);
                ticketItem.OnEndDrag = item => OnTicketItemEndDrag?.Invoke(ticket, item);
                ticketItem.titleText.text = ticket.title;

                currTicketItems.Add(ticketItem);
            }
        }

        public void SetTicketPlaceholderActive(bool value, int index = 0, float height = 35)
        {
            itemPlaceholder.gameObject.SetActive(value);
            itemPlaceholder.transform.SetSiblingIndex(index);
            itemPlaceholder.SetHeight(height);
        }

        public void TakeTicket(Ticket ticket, TicketItem ticketItem, int index)
        {
            ticketItem.transform.SetParent(itemParent);
            ticketItem.transform.SetSiblingIndex(index);
            ticketItem.itemButton.onClick.AddListener(() => OnTicketItemClick?.Invoke(ticket));
            ticketItem.OnDrag = item => OnTicketItemDrag?.Invoke(ticket, item);
            ticketItem.OnBeginDrag = item => OnTicketItemBeginDrag?.Invoke(ticket, item);
            ticketItem.OnEndDrag = item => OnTicketItemEndDrag?.Invoke(ticket, item);
            
            currTicketItems.Add(ticketItem);
        }
    }
}