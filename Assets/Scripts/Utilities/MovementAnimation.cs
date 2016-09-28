using UnityEngine;

namespace CustomAnimations
{
    public class MovementAnimation : MyAnimation
    {

        Vector3 startPoint;
        Vector3 targetPoint;

        public void Init(Vector3 targetPoint, float duration)
        {
            startPoint = gameObject.transform.localPosition;
            this.targetPoint = targetPoint;
            this.duration = duration;
            //Debug.Log(startPoint);
            //Debug.Log(targetPoint);
            //Debug.Log(transform.parent.gameObject);
        }

        public void UndoMovementAnimation()
        {
            //Debug.Log(startPoint);
            //Debug.Log(targetPoint);
            //Debug.Log(transform.parent.gameObject);
            StartReverseAnimating();
        }

        protected override void Animate(float completion)
        {
            transform.localPosition = startPoint * (1 - completion) + targetPoint * completion;
        }

        public static MovementAnimation CreateMovementAnimation(GameObject animatedObject, Vector3 targetPoint, float duration)
        {
            MovementAnimation animation = animatedObject.AddComponent<MovementAnimation>();
            animation.Init(targetPoint, duration);
            return animation;
        }

        public void SetDuration(float duration)
        {
            this.duration = duration;
        }


    }
}