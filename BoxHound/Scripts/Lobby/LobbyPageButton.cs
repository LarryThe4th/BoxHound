﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using BoxHound.Manager;


namespace BoxHound.Larry
{
    public class LobbyPageButton : MonoBehaviour {
        #region Public variables
        // -------------- Public variable -------------
        public int PageIndex;
        public LobbyUIBase.Pages RespondTo;
        public Transform DecorationBar;
        #endregion

        #region Private variables
        // -------------- Public variable -------------
        private Button m_Button;
        #endregion

        public void Selected(bool isSelected) {
            if (m_Button == null) {
                m_Button = GetComponentInChildren<Button>();
                m_Button.onClick.AddListener(delegate { OnClickedButton(); });
            }
            m_Button.interactable = isSelected;
            DecorationBar.gameObject.SetActive(!isSelected);
        }

        // Because normaly the unity3D button event wont take a enum argment,
        // We use a hacky way to bypass the limitation of toggle/button.
        public void OnClickedButton() {
            //if (LobbyManager.IgnoreUserInput) return;
            //LobbyManager.Instance.OnClickedPageButton(RespondTo);
        }
    }
}
