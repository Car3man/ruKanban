using System;
using System.Collections.Generic;
using RuKanban.Services.Api.JsonModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class WorkspaceSettingsWindow : BaseAppWindow
    {
        public Button closeButton;
        public TMP_InputField nameInput;
        public GameObject inviteUserItem;
        public TMP_InputField inviteUserLoginInput;
        public Button inviteUserButton;
        
        [SerializeField] private WorkspaceSettingsUserItem itemTemplate;
        [SerializeField] private Transform itemParent;

        private List<WorkspaceSettingsUserItem> _items;

        public Action<User> OnUserItemDeleteButtonClick;

        protected override void HideWindow(bool force = false)
        {
            ResetElements();
            
            base.HideWindow(force);
        }

        public override void ResetElements()
        {
            closeButton.onClick.RemoveAllListeners();
            nameInput.text = string.Empty;
            inviteUserLoginInput.text = string.Empty;
            inviteUserButton.onClick.RemoveAllListeners();
            itemTemplate.gameObject.SetActive(false);
            SetWorkspaceUsers(new List<User>());
            OnUserItemDeleteButtonClick = null;
        }

        public void SetWorkspaceUsers(List<User> users)
        {
            if (_items != null)
            {
                foreach (WorkspaceSettingsUserItem item in _items)
                {
                    Destroy(item.gameObject);
                }
            }

            _items = new List<WorkspaceSettingsUserItem>();
            
            foreach (User user in users)
            {
                WorkspaceSettingsUserItem item = Instantiate(itemTemplate, itemParent);
                item.gameObject.SetActive(true);
                
                item.loginText.text = user.login;
                item.deleteButton.onClick.AddListener(() => OnUserItemDeleteButtonClick?.Invoke(user));
                
                _items.Add(item);
            }
            
            inviteUserItem.transform.SetAsLastSibling();
        }
    }
}