
using System;
using CustomAnimations;
using UnityEngine;

namespace CustomAnimations
{
    public class ScaleAndDisappearAnimation : MyAnimation
    {
        private Vector3 initialScale;
        private float sizeBeginning;
        private float sizeEnd;
        private bool useBaseScale;

        protected override void Animate(float completion)
        {
            transform.localScale = initialScale * (completion * sizeEnd + (1 - completion) * sizeBeginning);
        }

        public void Init(float sizeBeginning, float sizeEnd, bool useBaseScale)
        {
            this.sizeEnd = sizeEnd;
            this.sizeBeginning = sizeBeginning;
            this.useBaseScale = useBaseScale;
        }

        protected override void StartAnimation(bool reset)
        {
            base.StartAnimation(reset);
            if (useBaseScale)
                initialScale = transform.localScale;
            else
                initialScale = Vector3.one;
        }

        public static ScaleAnimation CreateScaleAnimation(GameObject animatedObject, float sizeBeginning, float sizeEnd, float duration, bool useBaseScale = true)
        {
            ScaleAnimation animation = animatedObject.AddComponent<ScaleAnimation>();
            animation.Init(sizeBeginning, sizeEnd, useBaseScale);
            animation.duration = duration;
            return animation;
        }
    }
}