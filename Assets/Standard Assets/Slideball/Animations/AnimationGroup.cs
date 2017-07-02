using System.Collections.Generic;
using UnityEngine;

namespace CustomAnimations
{
    public delegate void AnimationGroupEventHandler(AnimationGroup sender);

    public class AnimationGroup : MonoBehaviour
    {

        public event AnimationGroupEventHandler FinishedAnimating;
        private List<MyAnimation> animations;
        private List<MyAnimation> executingAnimations;

        void Awake()
        {
            animations = new List<MyAnimation>();
            executingAnimations = new List<MyAnimation>();
        }

        public void AddAnimation(MyAnimation animation)
        {
            animations.Add(animation);
            animation.FinishedAnimating += new AnimationEventHandler(AnimationFinishedAnimating);
        }

        private void AnimationFinishedAnimating(MonoBehaviour sender)
        {
            MyAnimation animation = (MyAnimation)sender;
            if (executingAnimations.Remove(animation))
            {
                if (executingAnimations.Count == 0)
                {
                    RaiseFinishedAnimating();
                }
            }
            else
            {
                Debug.LogError(gameObject.name + "  - " + sender + ":  The animations should have been in the executing list");
            }
        }

        private void RaiseFinishedAnimating()
        {
            if (FinishedAnimating != null)
            {
                FinishedAnimating.Invoke(this);
            }
        }

        public void StartAnimating()
        {
            executingAnimations.Clear();
            foreach (MyAnimation animation in animations)
            {
                executingAnimations.Add(animation);
                animation.StartAnimating();
            }
        }

        public void StartReverseAnimating()
        {
            executingAnimations.Clear();
            foreach (MyAnimation animation in animations)
            {
                executingAnimations.Add(animation);
                animation.StartReverseAnimating();
            }
        }

        public void ChangeDuration(float newDuration)
        {
            foreach(MyAnimation animation in animations)
            {
                animation.duration = newDuration;
            }
        }

        void OnDestroy()
        {
            foreach (MyAnimation animation in animations)
            {
                Destroy(animation);
            }
        }
    }
}