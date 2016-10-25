using UnityEngine;
using System.Collections.Generic;
using System;

namespace BoxHound.UI
{
    public class DamageIndicatorManager : UIBase
    {
        #region Delegate and event
        public delegate void DamageIndicatorHandler(DamageIndicatorInfo info);
        public static DamageIndicatorHandler DamageIndicatorEvent;
        public static void NewDamageIndicator(DamageIndicatorInfo info) {
            if (DamageIndicatorEvent != null) DamageIndicatorEvent(info);
        }

        private void OnEnable()
        {
            // Subscribe the function to the event
            DamageIndicatorEvent += OnNewIndicator;
        }

        private void OnDisable()
        {
            // Unsubscribe the function form the event
            DamageIndicatorEvent -= OnNewIndicator;
        }
        #endregion

        #region Private variables
        // The list of indicators.
        private List<DamageIndicator> m_IndicatorList = new List<DamageIndicator>();
        // How long will the indicator last on screen.
        private float m_IndicatorDisplayDuration = 4;
        [Tooltip("The prefab of the damange indicator UI")]
        [SerializeField]
        private PoolObject IndicatorUIPerfab;
        private int m_PerfabInstanceID = 0;
        [Tooltip("RectTransform where indicators will be instantiate (Default Root Canvas)")]
        [SerializeField]
        private Transform IndicatorRoot;
        #endregion

        public override void InitUI()
        {
            base.InitUI();

            Properties = new UIProperties(
                UIframework.UIManager.SceneUIs.DamageHUDUI, 
                UIframework.UIManager.DisplayUIMode.Normal, 
                UIframework.UIManager.UITypes.UnderBlurEffect, 
                true, true);

            m_IndicatorList = new List<DamageIndicator>();
            m_IndicatorDisplayDuration = 4;
            m_PerfabInstanceID = 0;

            // Create a new pool in the object pooling manager for the indicators.
            ObjectPoolManager.Instance.CreaterPool(IndicatorUIPerfab, IndicatorRoot, 12);
            m_PerfabInstanceID = IndicatorUIPerfab.GetInstanceID();
        }

        /// <summary>
        /// When a damage has been done, a new event called
        /// </summary>
        /// <param name="info">Info about the new indicator</param>
        private void OnNewIndicator(DamageIndicatorInfo info) {
            // Determine if need create a new indicator or just update one existing.
            // Check if there is a indicator in the list which has a same attacker.
            if (m_IndicatorList.Exists(x => x.GetInfo.AttackerID == info.AttackerID)) {
                // Get the indicator's index form list.
                int index = GetIndicatorIndexFormList(info.AttackerID);
                if (index != -1) {
                    // If just update, delay the indicator's fadeout timer by the half of the display duration.
                    info.IndicatorDisplayDuration = m_IndicatorDisplayDuration / 2;
                    // Update the indicator.
                    UpdateIndicator(info, index);
                } else {
                    Debug.LogError("Couldn't find the indicator in the list, it should create one.");
                }
            }
            // If there is no such indicator that has a same attacker ID, then create one.
            else {
                // Set the indicator display duration.
                info.IndicatorDisplayDuration = m_IndicatorDisplayDuration;
                // In most of the cases the indicator sprite's color should not be BLACK
                // If its BLACK that means its not been set any colors yet
                if (info.IndicatorSpriteColor == Color.black) 
                    // Set it as RED.
                    info.IndicatorSpriteColor = Color.red;

                CreateIndicator(info);

                ControlIndicators();
            }
        }

        /// <summary>
        /// Create (request the object pooling manager for object reuse) a new indicator UI.
        /// </summary>
        /// <param name="info">The information about this new indicator.</param>
        private void CreateIndicator(DamageIndicatorInfo info)
        {
            // Reqest object reuse
            DamageIndicator indicator = (DamageIndicator)(ObjectPoolManager.Instance.ReuseObject(
                m_PerfabInstanceID, 
                info, this).m_Instance);

            // Add the active indicator into the list
            m_IndicatorList.Add(indicator);
        }

        /// <summary>
        /// Update a exist indicator.
        /// </summary>
        /// <param name="info">The indicator info</param>
        /// <param name="index">The index of inicator in the list.</param>
        private void UpdateIndicator(DamageIndicatorInfo info, int index) {
            DamageIndicator indicator = m_IndicatorList[index];
            if (indicator == null) {
                Debug.LogWarning("Can't update indicator because this does't exit in list");
                return;
            }
            if (info.IndicatorSpriteColor == Color.black)
                info.IndicatorSpriteColor = Color.red;
            // Update the exist indicator.
            indicator.UpdateIndicator(info, this);
        }

        /// <summary>
        /// When a indicator is completely invisible, remove it form the list.
        /// </summary>
        /// <param name="indicator">The indicator that is going to be remove.</param>
        public void RemoveIndicator(DamageIndicator indicator) {
            // Remove the indicator form the list
            if (!m_IndicatorList.Remove(indicator))
            {
                Debug.LogWarning("Indicator " + indicator.gameObject.name +
                    " is not in the list but the manager is tring to remove it.");
            }
        }

        public void ProcessIndicators() {
            if (m_IndicatorList.Count > 0) ControlIndicators();
        }

        /// <summary>
        /// Controll direction of each Indicator in List.
        /// </summary>
        private void ControlIndicators() {
            for (int index = 0; index < m_IndicatorList.Count; index++)
            {
                // Pass if the indicator is not been assgined.
                if (m_IndicatorList[index].GetAttackerID == -1) continue;

                DamageIndicator indicator = m_IndicatorList[index];

                //Remove nulls indicators in list
                if (indicator == null || indicator.transform == null) {
                    m_IndicatorList.Remove(indicator);
                    return;
                }

                // Get player camera facing direction
                Vector3 facing = CharacterManager.LocalPlayer.MainCamera.transform.forward;

                Vector3 rhs = indicator.GetInfo.Direction - CharacterManager.LocalPlayer.transform.position;
                Vector3 offset = indicator.transform.localEulerAngles;
                // Convert angle into screen space    
                rhs.y = 0f;
                rhs.Normalize();
                // Get the angle between two positions.
                float angle = Vector3.Angle(rhs, facing);
                // Calculate the perpendicular of both vectors
                Vector3 perpendicular = Vector3.Cross(facing, rhs);
                // Calculate magnitude between two vectors
                float dot = -Vector3.Dot(perpendicular, CharacterManager.LocalPlayer.transform.up);
                // Get the horinzontal angle in direction of target / attacker.
                angle = AngleCircumference(dot, angle);
                //Apply the horizontal rotation to the indicator.
                offset.z = angle;

                indicator.transform.localRotation = Quaternion.Euler(offset);
            }
        }

        public int GetIndicatorIndexFormList(int attackerID)
        {
            for (int index = 0; index < m_IndicatorList.Count; index++)
            {
                if (m_IndicatorList[index].GetAttackerID != -1)
                {
                    if (m_IndicatorList[index].GetAttackerID ==
                        attackerID)
                    {
                        return index;
                    }
                }
            }
            return -1;
        }

        private float AngleCircumference(float dot, float angle)
        {
            float ac = angle;
            float circumference = 360f;
            ac = angle - 10;
            if (dot < 0)
            {
                ac = circumference - angle;
            }
            return ac;
        }

        public override void SetLanguage(GameLanguageManager.SupportedLanguage language)
        {
            // Empty
        }
    }
}
