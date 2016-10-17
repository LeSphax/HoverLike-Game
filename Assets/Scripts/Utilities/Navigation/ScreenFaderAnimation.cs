using UnityEngine;

namespace CustomAnimations
{
    class ScreenFaderAnimation : SpriteColorAnimation
    {
        private AnimationCurve curve;

        public override void SetVariables(Color startColor, Color endColor, float duration)
        {
            base.SetVariables(startColor, endColor, duration);
            curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        protected override void Animate(float completion)
        {
            completion = curve.Evaluate(completion);
            base.Animate(completion);
        }

        public static SpriteColorAnimation CreateScreenFaderAnimation(GameObject animatedObject, Color startColor, Color targetColor, float duration)
        {
            ScreenFaderAnimation animation = animatedObject.AddComponent<ScreenFaderAnimation>();
            animation.SetVariables(startColor, targetColor, duration);
            return animation;
        }
    }
}
