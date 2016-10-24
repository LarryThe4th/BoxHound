using UnityEngine;
using System.Collections;

namespace BoxHound.UIFrameWork
{
    interface IAnimation
    {
        void EnterAnimation(Event onComplete);

        void QuitAniamtion(Event onComplete);

        void ResetAnimation();
    }

    interface IEventListener {
        void ReigistEvent();

        void UnReigistEvent();
    }
}
