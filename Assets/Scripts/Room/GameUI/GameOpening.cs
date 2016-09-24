using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Larry.BoxHound
{
    public class GameOpening : MonoBehaviour
    {
        #region Public variables
        // -------------- Public variable -------------
        public Transform ChooseTeam;
        public Text StartGameTips;
        public Text HeaderText;
        public Text RoomInfoText;

        public bool TeamSelected { get; private set; }
        #endregion

        #region Private variables
        // -------------- Private variable -------------

        #endregion

        public void Show(bool show)
        {
            this.gameObject.SetActive(show);
            if (show)
                UpdateGameIntroductionText();
            else
                RoomManager.Instance.StartGame();
        }

        public void Init(bool enableTeam) {
            Show(true);

            // Reset flag. If current game mode didn't use team system, 
            // then every player has been set as free by default.
            TeamSelected = !enableTeam;

            ChooseTeam.gameObject.SetActive(enableTeam);
            if (enableTeam)
            {
                StartGameTips.text = "Choose a team to start";
            }
            else {
                StartGameTips.text = "Press ENTER to start";
            }
        }

        public void OnSelectedTeam(string teamName) {
            if (TeamSelected) return;
            // Player sound effect.
            SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuConfirm);
            // Using PUN's implementation of Hashtable
            Hashtable team = new Hashtable();
            if (string.Compare(teamName, "Red") == 0)
            {
                team[PlayerProperties.Team] = (int)GameModeManager.Team.Red;
            }
            else {
                team[PlayerProperties.Team] = (int)GameModeManager.Team.Blue;
            }
            PhotonNetwork.player.SetCustomProperties(team);
            TeamSelected = true;
            Show(false);
        }

        public void OnClickStart() {
            // Player sound effect.
            SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuConfirm);
            Show(false);
        }

        public void UpdateGameIntroductionText()
        {
            // Get current room information form the server.
            Room currentRoom = PhotonNetwork.room;

            HeaderText.text = PhotonNetwork.room.name + " - " + GameMapManager.GetGameMap((int)currentRoom.customProperties[RoomProperties.MapIndex]).GameMapName;
            RoomInfoText.text = GameModeManager.GetGameModeDetail(
                    (int)currentRoom.customProperties[RoomProperties.GameModeIndex]).GameModeName.ToUpper() + ": " 
                    + (int)currentRoom.customProperties[RoomProperties.ScoreLimit] + " TICKETS " + (int)currentRoom.customProperties[RoomProperties.TimeLimit] + " MIN - TIME LEFT: 00:00";
            //m_Timer += Time.deltaTime;

            //var minutes = m_Timer / 60; //Divide the guiTime by sixty to get the minutes.
            //var seconds = m_Timer % 60;//Use the euclidean division for the seconds.

            ////update the label value
            //RoomInfoText.text = "Timer: " + minutes + ":" + seconds;
        }
    }
}
