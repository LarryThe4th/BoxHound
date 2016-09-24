using UnityEngine;
using System.Collections.Generic;

namespace Larry.BoxHound
{
    /// <summary>
    /// A base abstract class for all the game modes available in game.
    /// </summary>
    public abstract class GameModeBase : MonoBehaviour
    {
        public enum SpawnMode {
            Random,
            RedOnly,
            BlueOnly
        }

        #region Protected Variables
        // -------------- Protected variable -------------
        protected float TotalRoundTimeInSceond = -1.0f;     // Indicates how long does one round last.
        protected const float SEC_PER_MIN = 60.0f;          // A const variable for calculating the timer. 
        #endregion

        #region Private Variables
        // -------------- Private variable -------------
        //private float m_EndRoundTime = 0.0f;
        //private float m_LastRealTime = 0.0f;
        #endregion

        #region public abstract methods
        /// <summary>
        /// Returns the current running game mode.
        /// </summary>
        public abstract GameModeManager.GameModes GetGameMode();

        /// <summary>
        /// Setup a game mode and switch it on and off based on currently selected game mode.
        /// </summary>
        public abstract void Setup(bool setEnable);

        /// <summary>
        /// Returns TRUE if the game mode use team rules.
        /// </summary>
        public abstract bool IsUsingTeamRule();

        /// <summary>
        /// Use this method to get spawn for new comer or respawn character.
        /// </summary>
        /// <param name="mode">Spawn detail</param>
        /// <returns></returns>
        public abstract Transform GetSpawnPoint(SpawnMode mode);

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
            if (TotalRoundTimeInSceond <= 0)
            {
                if (roomDetail.customProperties.ContainsKey(RoomProperties.TimeLimit))
                {
                    TotalRoundTimeInSceond = (int)roomDetail.customProperties[RoomProperties.TimeLimit] * SEC_PER_MIN;
                }
                else
                {
                    Debug.LogError("The room's custom properties does not contains a TimeLimit key, check the CreateRoom related method.");
                }
            }
            else
            {
                Debug.LogError("The total round time is less than 0, game can't start whitout a timer.");
            }
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
                if (PhotonNetwork.room.customProperties.ContainsKey(RoomProperties.StartTime))
                {
                    timePassed = PhotonNetwork.time - (double)PhotonNetwork.room.customProperties[RoomProperties.StartTime];
                }
            }
            return timePassed >= TotalRoundTimeInSceond;
            #endregion
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
                updateProperties.Add(RoomProperties.StartTime, PhotonNetwork.time);
                // Update room property.
                PhotonNetwork.room.SetCustomProperties(updateProperties);
            }
        }
        #endregion
    }
}
