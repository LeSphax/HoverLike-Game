using UnityEngine;
using UnityEngine.UI;

namespace CustomAnimations
{
    public class GetComponentsFadeAnimation : MyAnimation
    {
        public const float timeCube = 0.2f;
        public bool cube;
        public bool disableWhenInvisible;

        Renderer[] renderersInChildren;
        Graphic[] graphicsInChildren;
        public float initialAlpha;
        public float inspectorDuration;

        void Awake()
        {
            Animate(initialAlpha);
            if (duration == 0)
            {
                Init();
            }
        }

        public void Init()
        {
            if (cube)
                duration = timeCube;
            else
                duration = inspectorDuration;
        }

        public void Init(float duration)
        {
            this.duration = duration;
        }

        public static GetComponentsFadeAnimation CreateGetComponentsFadeAnimation(GameObject animatedObject, float duration)
        {
            GetComponentsFadeAnimation animation = animatedObject.AddComponent<GetComponentsFadeAnimation>();
            animation.Init(duration);
            return animation;
        }

        public override void StartReverseAnimating(bool reset = false)
        {
            base.StartReverseAnimating(reset);
            //Debug.Log(gameObject.name +"  REVERSE "+ duration);
        }

        public override void StartAnimating(bool reset = false)
        {
            base.StartAnimating(reset);
            //Debug.Log(gameObject.name + "  ANIM  " + duration);
        }

        protected override void StartAnimation(bool reset)
        {
            base.StartAnimation(reset);
            if (disableWhenInvisible)
            {
                gameObject.SetActive(true);
            }
        }

        protected override void FinishAnimation()
        {
            bool disable = false;
            if (state == State.REVERSEANIMATING && disableWhenInvisible)
            {
                disable = true;
            }
            base.FinishAnimation();
            if (disable)
            {
                gameObject.SetActive(false);
            }
        }

        protected override void Animate(float completion)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                foreach (Material material in renderer.materials)
                {
                    if (material.HasProperty("_Color"))
                        material.color = new Color(material.color.r, material.color.g, material.color.b, completion);
                }
            }
            foreach (Graphic graphic in GetComponentsInChildren<Graphic>())
            {
                graphic.color = new Color(graphic.color.r, graphic.color.g, graphic.color.b, completion);
            }
        }
    }
}