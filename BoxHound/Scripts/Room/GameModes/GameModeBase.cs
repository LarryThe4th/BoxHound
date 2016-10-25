using UnityEngine;
using BoxHound.Manager;
using HashTable = ExitGames.Client.Photon.Hashtable;

namespace BoxHound
{
    /// <summary>
    /// A base abstract class for all the game modes available in game.
    /// </summary>
    public abstract class GameModeBase : MonoBehaviour
    {
        #region Public Variables
        // -------------- Public variable -------------
        public float GetRespawnTime {
            get { return RespawnCountDownTime; }
        }

        public float GetPreparationPhaseTotalTime {
            get  { return RoundPreparationPhaseTotalTime; }
        }
        #endregion

        #region Protected Variables
        // -------------- Protected variable -------------
        protected readonly float RoundPreparationPhaseTotalTime = 10.0f; // How long will the round preparation phase last, default value is 15 second.
        protected  bool PreparationPhaseEnded = false;                   // Check if the round preparation phase is ended.
        protected readonly float RespawnCountDownTime = 6.0f;            // How long will the respawn process take.

        protected readonly float EndGameResultTotalTime = 10.0f;        // After game ended, how long until auto load next round.
        protected float RemainRoundEndTime = -10.0f;

        protected float TotalRoundTime;                         // Indicates how long will one round last.
        protected double RoundTimePassed = 0.0;                 // How long did one round last since round started.
        protected const float SEC_PER_MIN = 60.0f;              // A const variable for calculating the timer. 
        #endregion

        #region Private Variables
        // -------------- Private variable -------------
        // Prevent form accidently setup multiple times.
        private bool m_SettedUp = false;
        #endregion

        #region public abstract methods
        /// <summary>
        /// Returns the current running game mode.
        /// </summary>
        public abstract GameModeManager.GameModes GetGameMode();

        /// <summary>
        /// Setup a game mode and switch it on and off based on currently selected game mode.
        /// </summary>
        public void Setup(bool enableGameMode) {
            if (!m_SettedUp) return;
            if (enableGameMode)
            {
                InitGameMode();
            }
            else {
                enabled = false;
            }            
            m_SettedUp = true;
        }

        /// <summary>
        /// Use this if need init game mode.
        /// </summary>
        protected virtual void InitGameMode() { }

        /// <summary>
        /// Returns TRUE if the game mode use team rules.
        /// </summary>
        public abstract bool IsUsingTeamRule();

        /// <summary>
        /// When a new round started, call this method for setup the game.
        /// </summary>
        /// <param name="roomDetail"></param>
        public virtual void StartNewRound(Room roomDetail) {
            // The master client stores its round start time, 
            // so that all clients can calculate themselves when the current round ends.
            SetRoundStartTime();

            // Calculate how long does one round last.
            // The total round time is the same as the other rounds as long as the game mode is the same.
            // So only update it when a new game mode is up running.
            if (TotalRoundTime <= 0)
            {
                // Get the pre-round time limit form room properties.
                if (roomDetail.customProperties.ContainsKey(RoomProperties.RoundTimeLimit))
                {
                    TotalRoundTime = (int)roomDetail.customProperties[RoomProperties.RoundTimeLimit] * SEC_PER_MIN;
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogError("The room's custom properties does not contains a TimeLimit key, check the CreateRoom related method.");
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("The total round time is less than 0, game can't start whitout a timer.");
            }
#endif
        }

        /// <summary>
        /// Returns TRUE when the round is finished by pre-set win condition.
        /// But no matter what if the timer hits ZERO the round will be ended.
        /// </summary>
        public virtual bool IsRoundFinished() {
            #region What ever win condition it is, when the time is up game will be over no matter what.
            double timePassed = 0;
            if (PhotonNetwork.room != null)
            {
                // PhotonNetwork.time is synchronized between all players. 
                // so we can be sure that each client gets the same result here.
                if (PhotonNetwork.room.customProperties.ContainsKey(RoomProperties.RoundStartTime))
                {
                    timePassed = PhotonNetwork.time - (double)PhotonNetwork.room.customProperties[RoomProperties.RoundStartTime];
                }
            }
            return timePassed >= TotalRoundTime;
            #endregion
        }

        /// <summary>
        /// Get spawn point for player object to respawn.
        /// </summary>
        /// <param name="team">Which team does the player object belong to.</param>
        /// <returns></returns>
        public abstract Transform GetSpawnPoint(CustomRoomOptions.Team team);

        /// <summary>
        /// Get preparation phase remain time.
        /// Return -1 when preparation phase time's up. Return -2 when error.
        /// </summary>
        /// <returns>Return -1 when preparation phase time's up. Return -2 when error.</returns>
        public double GetPreparationPhaseTimeLeft() {
            if (PreparationPhaseEnded) return -1;

            if (PhotonNetwork.room != null)
            {
                // If contains room property Round Start Time
                if (PhotonNetwork.room.customProperties.ContainsKey(RoomProperties.RoundStartTime))
                {
                    RoundTimePassed = PhotonNetwork.time - (double)PhotonNetwork.room.customProperties[RoomProperties.RoundStartTime];
                    // If preparation phase time's up
                    if ((RoundPreparationPhaseTotalTime - RoundTimePassed) <= 0) {
                        PreparationPhaseEnded = true;
                        return -1;
                    }
                    return RoundPreparationPhaseTotalTime - RoundTimePassed;
                }
            }
            return -2;
        }

        /// <summary>
        /// Get remain time since scene loaded in second.
        /// </summary>
        /// <param name="timePassed">How long since the scene loaded.</param>
        /// <returns></returns>
        public abstract double GetRoundTimeLeft();

        public void ToNextRound() {
            // Reset the remain round end timer.
            if (RemainRoundEndTime <= -5) {
                PhotonNetwork.automaticallySyncScene = true;
                RemainRoundEndTime = EndGameResultTotalTime;
            } 

            // Count Down until load next round
            RemainRoundEndTime -= Time.deltaTime;

            if (RemainRoundEndTime <= 0) {
                LoadNextRound();
            }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Set the round start time. 
        /// Use it to calculate how long does the game last since this round started.
        /// This method is for the master client only.
        /// </summary>
        protected void SetRoundStartTime() {
            if (PhotonNetwork.isMasterClient) {
                ExitGames.Client.Photon.Hashtable updateProperties = new ExitGames.Client.Photon.Hashtable();
                updateProperties.Add(RoomProperties.RoundStartTime, PhotonNetwork.time);

                // Update room property.
                PhotonNetwork.room.SetCustomProperties(updateProperties);
            }
        }

        /// <summary>
        /// Gets all the necessary data that is needed to load the next round or map.
        /// </summary>
        public void LoadNextRound()
        {
            // Only the master client can reload the scene
            if (PhotonNetwork.isMasterClient) {
                Room currentRoom = PhotonNetwork.room;
                HashTable resetProperties = new HashTable();
                // Game mode keep it as it is.
                resetProperties.Add(RoomProperties.GameModeIndex, (int)currentRoom.customProperties[RoomProperties.GameModeIndex]);
                // Reset the round start time.
                resetProperties.Add(RoomProperties.RoundStartTime, PhotonNetwork.time);
                // Round time limit won't change
                resetProperties.Add(RoomProperties.RoundTimeLimit, (int)currentRoom.customProperties[RoomProperties.RoundTimeLimit]);
                // Reset both TeamA's and TeamB's score.
                resetProperties.Add(RoomProperties.TeamAScore, 0);
                resetProperties.Add(RoomProperties.TeamBScore, 0);
                // The room name of cause is the same.
                resetProperties.Add(RoomProperties.RoomName, (string)currentRoom.customProperties[RoomProperties.RoomName]);
                // The mapIndex is the same.

                int nextMap = (int)currentRoom.customProperties[RoomProperties.MapIndex] + 1;
                if (nextMap >= GameMapManager.GetMapCount) nextMap = 0;

                resetProperties.Add(RoomProperties.MapIndex, nextMap);
                // The room create data won't change.
                resetProperties.Add(RoomProperties.RoomCreateDate, (double)currentRoom.customProperties[RoomProperties.RoomCreateDate]);

                resetProperties.Add(RoomProperties.HealthLimit, (int)currentRoom.customProperties[RoomProperties.HealthLimit]);

                // Reset these information in the rooms custom properties
                currentRoom.SetCustomProperties(resetProperties);

                // Reload the next round scene
                // PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex);
                PhotonNetwork.LoadLevel(GameMapManager.GetGameMap(nextMap).GameMapSceneIndex);
            }
        }
        #endregion
    }
}
