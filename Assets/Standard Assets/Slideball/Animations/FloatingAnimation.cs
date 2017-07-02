using UnityEngine;

namespace CustomAnimations.BallMazeAnimations
{
    public class FloatingAnimation : MyAnimation
    {
        [SerializeField]
        private float deltaHeight;

        private float previousCompletion;

        private AnimationCurve curve;

        private bool activated;

        void Start()
        {
            Init(base.duration, deltaHeight);
            StartAnimating();
        }

        public void Init(float duration, float deltaHeight)
        {
            Init();
            base.duration = duration;
            this.deltaHeight = deltaHeight;
            delay = 0;
        }

        private void Init()
        {
            curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        public void UndoMovementAnimation()
        {
            StartReverseAnimating();
        }

        protected override void Animate(float completion)
        {
            float positionCompletion = curve.Evaluate(completion);
            float deltaMovement = (positionCompletion - previousCompletion) * deltaHeight;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + deltaMovement, transform.localPosition.z);
            previousCompletion = positionCompletion;
        }

        protected override void FinishAnimation()
        {
            if (state == State.ANIMATING)
            {
                Animate(1);
                StartReverseAnimating();
            }
            else if (state == State.REVERSEANIMATING)
            {
                Animate(0);
                StartAnimating();
            }

        }

        public void Activate(bool activate)
        {
            if (activate && state == State.IDLE)
            {
                Animate(0);
                StartAnimating();
            }
            else if (!activate && state != State.IDLE)
            {
                state = State.IDLE;
                Animate(0);
            }
        }

        public static void AddFloatingAnimation(GameObject target, float duration, float deltaHeight)
        {
            FloatingAnimation animation = target.AddComponent<FloatingAnimation>();
            animation.Init(duration, deltaHeight);
        }
    }
}
