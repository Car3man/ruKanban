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
        [SerializeField] private Image highlightOverlay;

        private List<Ticket> _currTickets;
        private List<TicketItem> _items;

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
            highlightOverlay.gameObject.SetActive(false);
        }
        
        public void SetTickets(List<Ticket> tickets)
        {
            _currTickets = tickets;
            
            if (_items != null)
            {
                foreach (TicketItem item in _items)
                {
                    Destroy(item.gameObject);
                }
            }

            _items = new List<TicketItem>();
            
            foreach (Ticket ticket in tickets)
            {
                TicketItem ticketItem = Instantiate(itemTemplate, itemParent);
                ticketItem.gameObject.SetActive(true);

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

                _items.Add(ticketItem);
            }
        }

        public void SetHighlighted(bool value)
        {
            highlightOverlay.gameObject.SetActive(value);
        }

        public void TakeTicket(Ticket ticket, TicketItem ticketItem)
        {
            ticketItem.transform.SetParent(itemParent);
            ticketItem.itemButton.onClick.AddListener(() => OnTicketItemClick?.Invoke(ticket));
            ticketItem.OnDrag = item => OnTicketItemDrag?.Invoke(ticket, item);
            ticketItem.OnBeginDrag = item => OnTicketItemBeginDrag?.Invoke(ticket, item);
            ticketItem.OnEndDrag = item => OnTicketItemEndDrag?.Invoke(ticket, item);
            
            _items.Add(ticketItem);
        }
    }
}