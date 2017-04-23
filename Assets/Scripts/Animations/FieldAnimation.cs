
using UnityEngine;

namespace CustomAnimations
{
    [RequireComponent(typeof(Renderer))]
    public class FieldAnimation : MyAnimation
    {
        private float yBeginning = 0;
        private float yEnd;
        private Material oldMaterial;
        private Material FieldMaterial
        {
            get
            {
                return ResourcesGetter.FieldMaterial;
            }
        }

        protected override void Animate(float completion)
        {
            float y = completion * yEnd + (1 - completion) * yBeginning;
            FieldMaterial.SetFloat("_Y", y);
        }

        public void Init(FieldDirection direction)
        {
            yEnd = FieldMaterial.GetFloat("_Spacing");
            if (direction == FieldDirection.DOWN)
            {
                yEnd = -yEnd;
            }
        }

        public override void StartAnimating(bool reset = false)
        {
            base.StartAnimating(reset);
            if (!reset)
            {
                oldMaterial = GetComponent<Renderer>().material;
                GetComponent<Renderer>().material = FieldMaterial;
                FieldMaterial.SetColor("_ColorM", oldMaterial.color);
            }
        }

        public override void StopAnimating()
        {
            base.StopAnimating();
            GetComponent<Renderer>().material = oldMaterial;
        }

        protected override void FinishAnimation()
        {
            StartAnimating(true);
        }

        public static FieldAnimation Create(GameObject animatedObject, float duration, FieldDirection direction)
        {
            FieldAnimation animation = animatedObject.AddComponent<FieldAnimation>();
            animation.Init(direction);
            animation.duration = duration;
            return animation;
        }
    }

    public enum FieldDirection
    {
        UP,
        DOWN,
    }
}