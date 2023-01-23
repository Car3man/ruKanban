using System;
using System.Collections.Generic;
using RuKanban.Services.Api.JsonModel;
using UnityEngine;
using UnityEngine.UI;

namespace RuKanban.App.Window
{
    public class UserWorkspacesWindow : BaseAppWindow
    {
        public BaseHeader header;
        public Button addUserWorkspaceButton;
        
        [SerializeField] private UserWorkspaceItem itemTemplate;
        [SerializeField] private Transform itemParent;

        private List<UserWorkspaceItem> _items;

        public Action<Workspace> OnUserWorkspaceItemButtonClick;
        public Action<Workspace> OnUserWorkspaceItemSettingsButtonClick;

        protected override void HideWindow(bool force = false)
        {
            ResetElements();
            base.HideWindow(force);
        }

        public override void ResetElements()
        {
            header.ResetElements();
            addUserWorkspaceButton.onClick.RemoveAllListeners();
            itemTemplate.gameObject.SetActive(false);
            SetUserWorkspaces(new List<Workspace>());
            OnUserWorkspaceItemButtonClick = null;
            OnUserWorkspaceItemSettingsButtonClick = null;
        }

        public void SetUserWorkspaces(List<Workspace> userWorkspaces)
        {
            if (_items != null)
            {
                foreach (UserWorkspaceItem item in _items)
                {
                    Destroy(item.gameObject);
                }
            }

            _items = new List<UserWorkspaceItem>();
            
            foreach (Workspace workspace in userWorkspaces)
            {
                UserWorkspaceItem userWorkspaceItem = Instantiate(itemTemplate, itemParent);
                userWorkspaceItem.gameObject.SetActive(true);

                userWorkspaceItem.itemButton.onClick.AddListener(() => OnUserWorkspaceItemButtonClick?.Invoke(workspace));
                userWorkspaceItem.settingsButton.onClick.AddListener(() => OnUserWorkspaceItemSettingsButtonClick?.Invoke(workspace));
                userWorkspaceItem.nameText.text = workspace.name;

                _items.Add(userWorkspaceItem);
            }
            
            addUserWorkspaceButton.transform.SetAsLastSibling();
        }
    }
}