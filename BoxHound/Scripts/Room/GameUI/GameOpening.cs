using UnityEngine;
using UnityEngine.UI;

namespace BoxHound.UI
{
    public class GameOpening : RoomUIBase
    {
        #region Public variables
        // -------------- Public variable -------------
        public CanvasGroup ChooseTeam;

        public Button JoinRED;
        public Text JoinREDText;
        public Image JoinREDCTick;

        public Button JoinBLUE;
        public Text JoinBLUEText;
        public Image JoinBLUECTick;


        public Text StartGameTips;
        public Text HeaderText;
        public Text RoomInfoText;

        public bool TeamSelected { get; private set; }
        #endregion

        #region Private variables
        // Do this game mode use team mechanic.
        private bool m_UseTeam = false;
        #endregion

        public void Init(bool enableTeam) {
            // Reset flag. If current game mode didn't use team system, 
            // then every one for them self.
            TeamSelected = !enableTeam;
            m_UseTeam = enabled;

            if (enableTeam)
            {
                ChooseTeam.alpha = 1;
                ChooseTeam.blocksRaycasts = true;
                StartGameTips.text = "チームを選択してください";
            }
            else {
                ChooseTeam.alpha = 0;
                ChooseTeam.blocksRaycasts = false;
                StartGameTips.text = "間もなく始めます...";
            }
        }

        /// <summary>
        /// React when player selected a team
        /// </summary>
        /// <param name="teamName"></param>
        public void OnSelectedTeam(string teamName) {
            // If the player already selected a team.
            if (TeamSelected) return;
            // Player sound effect.
            SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuConfirm);
            // If the player wish to join the RED team
            if (string.Compare(teamName, "Red") == 0)
            {
                Helper.SetCustomPlayerProperty<int>(CharacterManager.LocalPlayer.photonView, PlayerProperties.Team, (int)CustomRoomOptions.Team.Red);
                CharacterManager.LocalPlayer.SetTeam(CustomRoomOptions.Team.Red);
            }
            else if (string.Compare(teamName, "Blue") == 0) {
                Helper.SetCustomPlayerProperty<int>(CharacterManager.LocalPlayer.photonView, PlayerProperties.Team, (int)CustomRoomOptions.Team.Blue);
                CharacterManager.LocalPlayer.SetTeam(CustomRoomOptions.Team.Blue);
            }
            TeamSelected = true;
        }

        public void OnClickStart() {
            // If current game mode use team mechanic but the player yet picked a team,
            // ignore the click.
            if (m_UseTeam && !TeamSelected) return;

            // If the Preparation phase is not yet ended, ignore the click.
            if (RoomManager.CurrentGameMode.GetPreparationPhaseTimeLeft() != -1) return;

            // Player sound effect.
            SoundManager.Instance.PlayUISFX(SoundManager.UISFX.MenuConfirm);

            DisplayUI(false);

            // Start the game
            RoomManager.Instance.ProcessGamePhase(RoomManager.GamePhase.RoundStart);
        }

        private void UpdateTips(bool enableTeam) {
            if (enableTeam) {
                // If preparation phase is ended, 
                // after player picked a team or just press the start button, join game immediate.
                if (RoomManager.CurrentGameMode.GetPreparationPhaseTimeLeft() == -1)
                {
                    // If player yet pick a team.
                    if (!TeamSelected)
                        StartGameTips.text = "SELECT A TEAM";
                    else
                        StartGameTips.text = "ENTERキを押してゲームを参加する";
                }
                // If the preparation phase is not yet ended.
                else
                {
                    // If player yet pick a team.
                    if (!TeamSelected)
                        StartGameTips.text = "SELECT A TEAM (" +
                            Mathf.FloorToInt((float)RoomManager.CurrentGameMode.GetPreparationPhaseTimeLeft()) + ")";
                    // If player picked a team but preparation phase not yet ended.
                    else
                        StartGameTips.text = "間もなく始めます (" +
                            Mathf.FloorToInt((float)RoomManager.CurrentGameMode.GetPreparationPhaseTimeLeft()) + ")";
                }
            }
            else {
                StartGameTips.text = "ENTERキを押してゲームを参加する";
            }
        }

        private void UpdateSelectTeamButton() {
            // If player yet pick a team
            if (!TeamSelected) {
                JoinREDCTick.enabled = false;
                JoinBLUECTick.enabled = false;
                // Check if the BLUE team is full
                if (RoomManager.CheckIfTeamFull(CustomRoomOptions.Team.Blue))
                {
                    JoinBLUEText.text = "Sorry\nBut BLUE have enough man.";
                    JoinBLUE.interactable = false;

                    JoinREDText.text = "JOIN RED RIGHT NOW!";
                    JoinRED.interactable = true;
                }
                // Check if the RED team is full
                else if (RoomManager.CheckIfTeamFull(CustomRoomOptions.Team.Blue)) {
                    JoinBLUEText.text = "Join BLUE and ensure victory!";
                    JoinBLUE.interactable = false;

                    JoinREDText.text = "The great RED don't need any more man.";
                    JoinRED.interactable = true;
                }
            } else {
                if (CharacterManager.LocalPlayer.Team == CustomRoomOptions.Team.Blue)
                {
                    JoinBLUE.GetComponentInChildren<Text>().text = "Good! The victory will be ours.";
                    JoinBLUE.interactable = true;
                    JoinBLUECTick.enabled = true;

                    JoinRED.GetComponentInChildren<Text>().text = "YOU MAKE A WRONG MOVE!";
                    JoinRED.interactable = false;
                    JoinREDCTick.enabled = false;
                }
                else if (CharacterManager.LocalPlayer.Team == CustomRoomOptions.Team.Red)
                {
                    JoinBLUE.GetComponentInChildren<Text>().text = "We respect your decision\nNOW DIED!";
                    JoinBLUE.interactable = false;
                    JoinBLUECTick.enabled = false;

                    JoinRED.GetComponentInChildren<Text>().text = "We will clash the BLUEs in no time!";
                    JoinRED.interactable = true;
                    JoinREDCTick.enabled = true;
                }
            }
        }

        private void UpdateGameIntroductionText()
        {
            // Get current room information form the server.
            Room currentRoom = PhotonNetwork.room;

            double timeLeft = RoomManager.CurrentGameMode.GetRoundTimeLeft();
            int minutesLeft = Mathf.FloorToInt((float)timeLeft / 60);
            int secondsLeft = Mathf.FloorToInt((float)timeLeft) % 60;

            if (minutesLeft <= 0 && secondsLeft <= 0) {
                RoomManager.Instance.ProcessGamePhase(RoomManager.GamePhase.RoundEnded);
                DisplayUI(false);
            }

            HeaderText.text = PhotonNetwork.room.name + " - " + GameMapManager.GetGameMap((int)currentRoom.customProperties[RoomProperties.MapIndex]).GameMapName;
            RoomInfoText.text = GameModeManager.GetGameModeDetail(
                    (int)currentRoom.customProperties[RoomProperties.GameModeIndex]).GameModeName.ToUpper() + ": "  + 
                    (int)currentRoom.customProperties[RoomProperties.RoundTimeLimit] + " 分 - 残り時間: " +
                    minutesLeft.ToString() + ":" + secondsLeft.ToString("00");
        }

        public void Process() {
            if (!m_IsDisplaying) return;
            UpdateSelectTeamButton();
            UpdateGameIntroductionText();
            UpdateTips(m_UseTeam);
        }

        public override GameRoomUI.RoomUITypes GetRoomUIType()
        {
            return GameRoomUI.RoomUITypes.GameOpening;
        }
    }
}
