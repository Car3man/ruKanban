using System;
using System.Collections.Generic;
using RuKanban.Services.Api.JsonModel;
using RuKanban.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class BoardWindow : BaseAppWindow
    {
        public BoardWindowHeader header;
        public Button addColumnButton;
        public RectTransform dragAndDropParent;
        
        [SerializeField] private GameObject itemTemplate;
        [SerializeField] private Transform itemParent;
        [SerializeField] private GameObject addColumnContainer;

        private List<Column> _currColumns;
        private List<GameObject> _items;

        private bool _isCurrDragTicket;
        private (Column, ColumnItem, Ticket, TicketItem) _currDragTicket;
        private Vector2 _currDragTicketOldAnchorMin;
        private Vector2 _currDragTicketOldAnchorMax;
        private Transform _currDragTicketOldParent;
        private int _currDragTicketOldSiblingIndex;
        private ColumnItem _currDragTicketColumnOver;

        public Action<Column, ColumnItem> OnColumnItemReady;
        public Action<Column, Ticket> OnColumnTicketClick;
        public Action<Column, Ticket> OnColumnTicketMoveToAnotherColumn;
        public Action<Column, ColumnItem> OnDeleteButtonClick;
        public Action<Column, ColumnItem> OnAddTicketButtonClick;

        protected override void HideWindow(bool force = false)
        {
            ResetElements();
            base.HideWindow(force);
        }

        public override void ResetElements()
        {
            header.ResetElements();
            addColumnButton.onClick.RemoveAllListeners();
            itemTemplate.gameObject.SetActive(false);
            SetColumns(new List<Column>());
            OnColumnTicketClick = null;
            OnColumnTicketMoveToAnotherColumn = null;
            OnDeleteButtonClick = null;
            OnAddTicketButtonClick = null;
            _isCurrDragTicket = false;
            _currDragTicket = default;
        }

        public void SetColumns(List<Column> columns)
        {
            _currColumns = columns;
            
            if (_items != null)
            {
                foreach (GameObject item in _items)
                {
                    Destroy(item);
                }
            }

            _items = new List<GameObject>();
            
            foreach (Column column in columns)
            {
                GameObject columnItemContainer = Instantiate(itemTemplate, itemParent);
                columnItemContainer.SetActive(true);
                
                ColumnItem columnItem = columnItemContainer.GetComponentInChildren<ColumnItem>();
                columnItem.gameObject.SetActive(true);

                columnItem.ResetElements();
                columnItem.nameText.text = column.name;
                columnItem.OnTicketItemClick = ticket => { OnColumnTicketClick?.Invoke(column, ticket); };
                columnItem.OnTicketItemDrag = (ticket, ticketItem) => { OnTicketItemDrag(column, columnItem, ticket, ticketItem); };
                columnItem.OnTicketItemBeginDrag = (ticket, ticketItem) => { OnTicketItemBeginDrag(column, columnItem, ticket, ticketItem); };
                columnItem.OnTicketItemEndDrag = (ticket, ticketItem) => { OnTicketItemEndDrag(column, columnItem, ticket, ticketItem); };
                columnItem.deleteButton.onClick.AddListener(() => OnDeleteButtonClick?.Invoke(column, columnItem));
                columnItem.addTicketButton.onClick.AddListener(() => OnAddTicketButtonClick?.Invoke(column, columnItem));

                _items.Add(columnItemContainer);
                
                OnColumnItemReady?.Invoke(column, columnItem);
            }
            
            addColumnContainer.transform.SetAsLastSibling();
        }
        
        private void OnTicketItemDrag(Column previousColumn, ColumnItem previousColumnItem, Ticket ticket, TicketItem ticketItem)
        {
            if (_isCurrDragTicket)
            {
                var dragTicketOverlapTrigger = _currDragTicket.Item4.overlapTrigger;
                
                var dragTicketRT = _currDragTicket.Item4.GetComponent<RectTransform>();
                dragTicketRT.anchorMin = Vector2.one / 2f;
                dragTicketRT.anchorMax = Vector2.one / 2f;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(dragAndDropParent, Input.mousePosition,
                    null, out var dragTicketPoint);
                dragTicketRT.anchoredPosition = dragTicketPoint;
                
                foreach (GameObject columnContainer in _items)
                {
                    var anotherColumnItem = columnContainer.GetComponentInChildren<ColumnItem>();
                    var anotherColumnItemRT = anotherColumnItem.GetComponent<RectTransform>();
                    bool isOverColumn = dragTicketOverlapTrigger.Overlaps(anotherColumnItemRT);
                    anotherColumnItem.SetHighlighted(isOverColumn);

                    if (isOverColumn)
                    {
                        _currDragTicketColumnOver = anotherColumnItem;
                    }
                }
            }
        }

        private void OnTicketItemBeginDrag(Column previousColumn, ColumnItem previousColumnItem, Ticket ticket, TicketItem ticketItem)
        {
            if (!_isCurrDragTicket)
            {
                _isCurrDragTicket = true;
                _currDragTicket = (previousColumn, previousColumnItem, ticket, ticketItem);
                
                var dragTicketRT = _currDragTicket.Item4.GetComponent<RectTransform>();
                _currDragTicketOldAnchorMin = dragTicketRT.anchorMin;
                _currDragTicketOldAnchorMax = dragTicketRT.anchorMax;
                _currDragTicketOldParent = dragTicketRT.parent;
                _currDragTicketOldSiblingIndex = dragTicketRT.GetSiblingIndex();
                
                dragTicketRT.SetParent(dragAndDropParent);
            }
        }

        private void OnTicketItemEndDrag(Column previousColumn, ColumnItem previousColumnItem, Ticket ticket, TicketItem ticketItem)
        {
            if (_isCurrDragTicket)
            {
                foreach (GameObject columnContainer in _items)
                {
                    var anotherColumnItem = columnContainer.GetComponentInChildren<ColumnItem>();
                    anotherColumnItem.SetHighlighted(false);
                }
                
                var dragTicketRT = _currDragTicket.Item4.GetComponent<RectTransform>();
                dragTicketRT.anchorMin = _currDragTicketOldAnchorMin;
                dragTicketRT.anchorMax = _currDragTicketOldAnchorMax;

                if (_currDragTicketColumnOver == null || _currDragTicketColumnOver == previousColumnItem)
                {
                    dragTicketRT.SetParent(_currDragTicketOldParent);
                    dragTicketRT.SetSiblingIndex(_currDragTicketOldSiblingIndex);
                }
                else
                {
                    int overColumnIndex = _items.IndexOf(_currDragTicketColumnOver.transform.parent.gameObject);
                    Column overColumn = _currColumns[overColumnIndex];
                    _currDragTicketColumnOver.TakeTicket(_currDragTicket.Item3, _currDragTicket.Item4);
                    OnColumnTicketMoveToAnotherColumn?.Invoke(overColumn, _currDragTicket.Item3);
                }

                _currDragTicket = default;
                _isCurrDragTicket = false;
            }
        }
    }
}