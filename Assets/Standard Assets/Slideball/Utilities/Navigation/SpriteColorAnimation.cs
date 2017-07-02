using UnityEngine;
using UnityEngine.UI;

namespace CustomAnimations
{

    [RequireComponent(typeof(Material))]
    public class SpriteColorAnimation : MyAnimation
    {

        Color startColor;
        Color endColor;
        Image myRenderer;

        public virtual void SetVariables(Color startColor, Color endColor, float duration)
        {
            myRenderer = GetComponent<Image>();
            this.startColor = startColor;
            this.endColor = endColor;
            this.duration = duration;
        }

        protected override void Animate(float completion)
        {
            myRenderer.color = startColor * (1 - completion) + endColor * completion;
        }

        public static SpriteColorAnimation CreateSpriteColorAnimation(GameObject animatedObject, Color startColor, Color targetColor, float duration)
        {
            SpriteColorAnimation animation = animatedObject.AddComponent<SpriteColorAnimation>();
            animation.SetVariables(startColor, targetColor, duration);
            return animation;
        }


    }
}
