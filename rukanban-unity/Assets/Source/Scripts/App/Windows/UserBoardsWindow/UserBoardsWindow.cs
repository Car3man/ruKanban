using System;
using System.Collections.Generic;
using RuKanban.Services.Api.DatabaseModels;
using UnityEngine;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class UserBoardsWindow : BaseAppWindow
    {
        public UserBoardsWindowHeader header;
        public Button addUserBoardButton;
        
        [SerializeField] private UserBoardItem itemTemplate;
        [SerializeField] private Transform itemParent;

        private List<UserBoardItem> _items;

        public Action<Board> OnUserBoardItemButtonClick;

        public override void ResetElements()
        {
            header.ResetElements();
            addUserBoardButton.onClick.RemoveAllListeners();
            itemTemplate.gameObject.SetActive(false);
            SetUserBoards(new List<Board>());
            OnUserBoardItemButtonClick = null;
        }

        protected override void HideWindow(bool force)
        {
            ResetElements();
            
            base.HideWindow(force);
        }

        public void SetUserBoards(List<Board> userBoards)
        {
            if (_items != null)
            {
                foreach (UserBoardItem item in _items)
                {
                    Destroy(item.gameObject);
                }
            }

            _items = new List<UserBoardItem>();
            
            foreach (Board board in userBoards)
            {
                UserBoardItem userBoardItem = Instantiate(itemTemplate, itemParent);
                userBoardItem.gameObject.SetActive(true);

                userBoardItem.itemButton.onClick.AddListener(() => OnUserBoardItemButtonClick?.Invoke(board));
                userBoardItem.nameText.text = board.name;
                userBoardItem.descriptionText.text = board.description;

                _items.Add(userBoardItem);
            }
            
            addUserBoardButton.transform.SetAsLastSibling();
        }
    }
}