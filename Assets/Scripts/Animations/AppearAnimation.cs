
using System;
using CustomAnimations;
using UnityEngine;

namespace CustomAnimations
{
    [RequireComponent(typeof(Renderer))]
    public class AppearAnimation : MyAnimation
    {
        private float yBeginning;
        private float yEnd;
        private Material material;

        protected override void Animate(float completion)
        {
            float y = completion * yEnd + (1 - completion) * yBeginning;
            material.SetFloat("_Y", y);
        }

        public void Init(float yBeginning, float yEnd)
        {
            this.yEnd = yEnd;
            this.yBeginning = yBeginning;
        }

        protected override void StartAnimation(bool reset)
        {
            base.StartAnimation(reset);
            material = GetComponent<Renderer>().material;
        }

        public static AppearAnimation Create(GameObject animatedObject, float yBeginning, float yEnd, float duration)
        {
            AppearAnimation animation = animatedObject.AddComponent<AppearAnimation>();
            animation.Init(yBeginning, yEnd);
            animation.duration = duration;
            return animation;
        }
    }
}