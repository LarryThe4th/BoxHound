using UnityEngine;
using UnityEngine.UI;

namespace BoxHound.Larry
{
    public class FloatingNumber : PoolObject
    {
        private Animation m_Animation;
        private Text m_FloatingText;

        public override void Init()
        {
            m_Animation = GetComponentInChildren<Animation>();
            if (!m_Animation) Debug.LogError("Something goes wrong with ths animation in FloatingNumber.");

            m_FloatingText = GetComponentInChildren<Text>();
            m_FloatingText.enabled = false;
        }

        public override void OnObjectReuse(params object[] options)
        {
            m_FloatingText.text = ((int)options[0]).ToString();
            m_Animation.Play();
        }
    }
}
