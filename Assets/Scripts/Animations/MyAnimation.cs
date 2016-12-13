using UnityEngine;

namespace CustomAnimations
{
    public delegate void AnimationEventHandler(MyAnimation sender);

    public abstract class MyAnimation : MonoBehaviour
    {

        public bool IsCompleted
        {
            get
            {
                Debug.Log(state);
                return state == State.COMPLETED;
            }
        }

        protected enum State
        {
            IDLE,
            COMPLETED,
            ANIMATING,
            REVERSEANIMATING,
            WAITING_FOR_ANIMATION,
            WAITING_FOR_REVERSEANIMATION,
        }

        protected State state = State.IDLE;
        public float duration;
        //An optional delay before the animation starts
        protected float delay;

        //The time the animation will really last when taking into account how much of the animation was already completed at the startingTime
        protected float realDuration;
        //The time when the StartAnimating method was called
        private float startingTime;

        //The last computed completion
        private float lastCompletion = 1;

        public event AnimationEventHandler FinishedAnimating;

        /**
         * Play the animation starting with a completion parameter of 0 and ending with 1
         * If the animation was already playing and the reset parameter is set to false, it starts at the last known completion instead of 0
         **/
        public virtual void StartAnimating(bool reset = false)
        {
            state = State.WAITING_FOR_ANIMATION;
            StartAnimation(reset);
        }

        /**
         * Play the animation in reverse order, starting with a completion parameter of 1 and ending with 0
         * If the animation was already playing and the reset parameter is set to false, it starts at the last known completion instead of 1
         **/
        public virtual void StartReverseAnimating(bool reset = false)
        {
            state = State.WAITING_FOR_REVERSEANIMATION;
            StartAnimation(reset);
        }

        protected virtual void StartAnimation(bool reset)
        {
            realDuration = _InitRealDuration(reset);
            startingTime = Time.time;
        }

        public virtual void StopAnimating()
        {
            state = State.IDLE;
        }

        protected virtual void Update()
        {
            float completion = -1;
            switch (state)
            {
                case State.IDLE:
                case State.COMPLETED:
                    completion = -1;
                    break;
                case State.WAITING_FOR_ANIMATION:
                    CheckTime(State.ANIMATING);
                    break;
                case State.WAITING_FOR_REVERSEANIMATION:
                    CheckTime(State.REVERSEANIMATING);
                    break;
                case State.ANIMATING:
                    completion = GetAnimationCompletion();
                    break;
                case State.REVERSEANIMATING:
                    completion = 1 - GetAnimationCompletion();
                    break;
                default:
                    completion = -1;
                    break;
            }
            if (completion == -1)
                return;
            else if (completion > 1 || completion < 0)
            {
                FinishAnimation();
                if (FinishedAnimating != null)
                    FinishedAnimating.Invoke(this);
            }
            else
            {
                Animate(completion);
                lastCompletion = GetAnimationCompletion();
            }

        }

        private void CheckTime(State newState)
        {
            if (Time.time >= (startingTime + delay))
            {
                state = newState;
                Update();
            }
        }

        /**
         * This method is where the actual animation happens, it is called every frame with a new completion.
         * The completion is the proportion of the time that happened since the beginning of the animation over the total duration of the animation
         **/
        protected abstract void Animate(float completion);

        protected virtual void FinishAnimation()
        {
            if (state == State.ANIMATING)
            {
                Animate(1);
                state = State.COMPLETED;
            }
            else if (state == State.REVERSEANIMATING)
            {
                Animate(0);
                state = State.IDLE;
            }
        }

        protected virtual float GetAnimationCompletion()
        {
            return (Time.time - (startingTime + delay) + (duration - realDuration)) / duration;
        }

        private float _InitRealDuration(bool reset)
        {
            if (reset)
            {
                return duration;
            }
            return InitRealDuration();
        }

        protected virtual float InitRealDuration()
        {
            if (lastCompletion >= 1 || lastCompletion <= 0)
                return duration;
            else
                return duration * lastCompletion;
        }

    }
}

