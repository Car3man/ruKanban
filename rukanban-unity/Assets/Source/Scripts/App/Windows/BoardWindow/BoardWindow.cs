using System;
using System.Collections.Generic;
using RuKanban.Services.Api.DatabaseModels;
using RuKanban.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// ReSharper disable All

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
        
        public List<ColumnItem> currColumnItems;

        private bool _isCurrDragTicket;
        private (ColumnItem, TicketItem) _currDragTicket;
        private Vector2 _currDragTicketOldAnchorMin;
        private Vector2 _currDragTicketOldAnchorMax;
        private Transform _currDragTicketOldParent;
        private int _currDragTicketOldSiblingIndex;
        private ColumnItem _currDragTicketColumnOver;

        public delegate void ColumnItemReadyDelegate(ColumnItem columnItem, Column column, bool isLocal);
        public delegate void ColumnTicketClickDelegate(TicketItem ticketItem);
        public delegate void ColumnTicketMoveDelegate(ColumnItem oldColumnItem, TicketItem ticketItem,
            ColumnItem newColumnItem, TicketItem insertAfterItem);
        public delegate void ColumnTitleChangeDelegate(ColumnItem columnItem, string newTitle);
        public delegate void ColumnDeleteButtonClickDelegate(ColumnItem columnItem);
        public delegate void ColumnAddTicketButtonClickDelegate(ColumnItem columnItem);
        
        public ColumnItemReadyDelegate OnColumnItemReady;
        public ColumnTicketClickDelegate OnColumnTicketClick;
        public ColumnTicketMoveDelegate OnColumnTicketMove;
        public ColumnTitleChangeDelegate OnColumnTitleChange;
        public ColumnDeleteButtonClickDelegate OnColumnDeleteButtonClick;
        public ColumnAddTicketButtonClickDelegate OnColumnAddTicketButtonClick;

        protected override void HideWindow(bool force)
        {
            ResetElements();
            base.HideWindow(force);
        }

        public override void ResetElements()
        {
            header.ResetElements();
            addColumnButton.onClick.RemoveAllListeners();
            
            itemTemplate.gameObject.SetActive(false);
            SetColumns(Array.Empty<Column>());
            
            _isCurrDragTicket = default;
            _currDragTicket = default; 
            _currDragTicketOldAnchorMin = default; 
            _currDragTicketOldAnchorMax = default; 
            _currDragTicketOldParent = default;
            _currDragTicketOldSiblingIndex = default;
            _currDragTicketColumnOver = default;
        
            OnColumnItemReady = null;
            OnColumnTicketClick = null;
            OnColumnTicketMove = null;
            OnColumnTitleChange = null;
            OnColumnDeleteButtonClick = null;
            OnColumnAddTicketButtonClick = null;
        }

        public void SetColumns(Column[] columns)
        {
            if (currColumnItems != null)
            {
                foreach (ColumnItem item in currColumnItems)
                {
                    Destroy(item.transform.parent.gameObject);
                }
            }

            currColumnItems = new List<ColumnItem>();
            
            foreach (Column column in columns)
            {
                ColumnItem columnItem = CreateColumn(column);
                OnColumnItemReady?.Invoke(columnItem, column, false);
            }
            
            addColumnContainer.transform.SetAsLastSibling();
        }

        public void CreateColumnLocal(string title)
        {
            Column column = new Column
            {
                title = title
            };
            ColumnItem columnItem = CreateColumn(column);
            OnColumnItemReady?.Invoke(columnItem, column, true);
        }

        public void DeleteColumn(ColumnItem columnItem)
        {
            currColumnItems.Remove(columnItem);
            Destroy(columnItem.transform.parent.gameObject);
        }

        private ColumnItem CreateColumn(Column column)
        {
            GameObject columnItemContainer = Instantiate(itemTemplate, itemParent);
            columnItemContainer.transform.SetSiblingIndex(columnItemContainer.transform.GetSiblingIndex() - 1);
            columnItemContainer.SetActive(true);
                
            ColumnItem columnItem = columnItemContainer.GetComponentInChildren<ColumnItem>();
            columnItem.gameObject.SetActive(true);
            
            columnItem.ResetElements();
            columnItem.titleText.text = column.title;
            columnItem.titleButton.onClick.AddListener(() => OnColumnTitleButtonClick(columnItem));
            columnItem.editTitleText.onEndEdit.AddListener((newTitle) => OnColumnTitleInputEndEdit(columnItem, newTitle));
            columnItem.deleteButton.onClick.AddListener(() => OnColumnDeleteButtonClick?.Invoke(columnItem));
            columnItem.addTicketButton.onClick.AddListener(() => OnColumnAddTicketButtonClick?.Invoke(columnItem));
            columnItem.OnTicketItemClick = ticketItem => { OnColumnTicketClick?.Invoke(ticketItem); };
            columnItem.OnTicketItemDrag = ticketItem => { OnTicketItemDrag(columnItem, ticketItem); };
            columnItem.OnTicketItemBeginDrag = ticketItem => { OnTicketItemBeginDrag(columnItem, ticketItem); };
            columnItem.OnTicketItemEndDrag = ticketItem => { OnTicketItemEndDrag(columnItem, ticketItem); };

            currColumnItems.Add(columnItem);

            return columnItem;
        }

        private void OnColumnTitleButtonClick(ColumnItem columnItem)
        {
            columnItem.titleText.gameObject.SetActive(false);
            columnItem.deleteButton.gameObject.SetActive(false);
            columnItem.editTitleText.gameObject.SetActive(true);
            columnItem.editTitleText.text = columnItem.titleText.text;
            EventSystem.current.SetSelectedGameObject(columnItem.editTitleText.gameObject);
        }

        private void OnColumnTitleInputEndEdit(ColumnItem columnItem, string newTitle)
        {
            columnItem.titleText.gameObject.SetActive(true);
            columnItem.deleteButton.gameObject.SetActive(true);
            columnItem.editTitleText.gameObject.SetActive(false);
            OnColumnTitleChange?.Invoke(columnItem, newTitle);
        }

        private void OnTicketItemBeginDrag(ColumnItem previousColumnItem, TicketItem ticketItem)
        {
            if (!_isCurrDragTicket)
            {
                _isCurrDragTicket = true;
                _currDragTicket = (previousColumnItem, ticketItem);
                
                var dragTicketRT = _currDragTicket.Item2.GetComponent<RectTransform>();
                _currDragTicketOldAnchorMin = dragTicketRT.anchorMin;
                _currDragTicketOldAnchorMax = dragTicketRT.anchorMax;
                _currDragTicketOldParent = dragTicketRT.parent;
                _currDragTicketOldSiblingIndex = dragTicketRT.GetSiblingIndex();
                
                dragTicketRT.SetParent(dragAndDropParent);
            }
        }

        private void OnTicketItemDrag(ColumnItem previousColumnItem, TicketItem ticketItem)
        {
            if (_isCurrDragTicket)
            {
                TicketItem dragTicket = _currDragTicket.Item2;
                
                var dragTicketOverlapTrigger = dragTicket.overlapTrigger;
                
                var dragTicketRT = dragTicket.GetComponent<RectTransform>();
                dragTicketRT.anchorMin = Vector2.one / 2f;
                dragTicketRT.anchorMax = Vector2.one / 2f;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(dragAndDropParent, Input.mousePosition,
                    null, out var dragTicketPoint);
                dragTicketRT.anchoredPosition = dragTicketPoint;
                
                foreach (ColumnItem anotherColumnItem in currColumnItems)
                {
                    var anotherColumnItemRT = anotherColumnItem.GetComponent<RectTransform>();
                    bool isOverColumn = dragTicketOverlapTrigger.Overlaps(anotherColumnItemRT);

                    if (isOverColumn)
                    {
                        if (_currDragTicketColumnOver != null && _currDragTicketColumnOver != anotherColumnItem)
                        {
                            _currDragTicketColumnOver.SetTicketPlaceholderActive(false);
                        }
                        
                        CalculateTicketIndexInColumn(dragTicket, anotherColumnItem, out var standAfterItem);
                        int siblingIndex = standAfterItem != null ? standAfterItem.transform.GetSiblingIndex() + 1 : 0;
                        
                        _currDragTicketColumnOver = anotherColumnItem;
                        _currDragTicketColumnOver.SetTicketPlaceholderActive(true, siblingIndex, ticketItem.Height);
                    }
                }
            }
        }

        private void OnTicketItemEndDrag(ColumnItem previousColumnItem, TicketItem ticketItem)
        {
            if (_isCurrDragTicket)
            {
                var dragTicket = _currDragTicket.Item2;
                var dragTicketRT = dragTicket.GetComponent<RectTransform>();

                foreach (ColumnItem columnItem in currColumnItems)
                {
                    columnItem.SetTicketPlaceholderActive(false);
                }

                if (_currDragTicketColumnOver == null)
                {
                    dragTicketRT.SetParent(_currDragTicketOldParent);
                    dragTicketRT.SetSiblingIndex(_currDragTicketOldSiblingIndex);
                }
                else
                {
                    CalculateTicketIndexInColumn(dragTicket, _currDragTicketColumnOver, out var standAfterItem);
                    OnColumnTicketMove?.Invoke(previousColumnItem, dragTicket, _currDragTicketColumnOver, standAfterItem);
                }
                
                dragTicketRT.anchorMin = _currDragTicketOldAnchorMin;
                dragTicketRT.anchorMax = _currDragTicketOldAnchorMax;

                _currDragTicket = default;
                _isCurrDragTicket = false;
            }
        }

        private void CalculateTicketIndexInColumn(TicketItem ticketItem, ColumnItem columnItem, out TicketItem standAfterItem)
        {
            var ticketItemRT = ticketItem.GetComponent<RectTransform>();
            var ticketItemParentRT = columnItem.TicketItemsParent.GetComponent<RectTransform>();

            var ticketItemWorldCorners = new Vector3[4];
            ticketItemRT.GetWorldCorners(ticketItemWorldCorners);
            
            var ticketItemParentWorldCorners = new Vector3[4];
            ticketItemParentRT.GetWorldCorners(ticketItemParentWorldCorners);

            float ticketCenter = (ticketItemWorldCorners[0].y + ticketItemWorldCorners[1].y) / 2f;
            
            standAfterItem = null;
            
            for (var i = columnItem.currTicketItems.Count - 1; i >= 0; i--)
            {
                var columnTicketItem = columnItem.currTicketItems[i];
                var columnTicketItemRT = columnTicketItem.GetComponent<RectTransform>();
                
                var columnTicketItemWorldCorners = new Vector3[4];
                columnTicketItemRT.GetWorldCorners(columnTicketItemWorldCorners);

                float columnTicketItemCenter = (columnTicketItemWorldCorners[0].y + columnTicketItemWorldCorners[1].y) / 2f;

                if (ticketCenter < columnTicketItemCenter)
                {
                    standAfterItem = columnTicketItem;
                    break;
                }
            }
        }
    }
}