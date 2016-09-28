
using System;
using CustomAnimations;
using UnityEngine;

public class ScaleAnimation : MyAnimation
{
    private Vector3 initialScale;
    private float sizeBeginning;
    private float sizeEnd;

    protected override void Animate(float completion)
    {
        transform.localScale = initialScale * (completion * sizeEnd + (1 - completion) * sizeBeginning);
    }

    public void Init(float sizeBeginning, float sizeEnd)
    {
        this.sizeEnd = sizeEnd;
        this.sizeBeginning = sizeBeginning;
    }

    protected override void StartAnimation(bool reset)
    {
        base.StartAnimation(reset);
        initialScale = transform.localScale;
    }

    public static ScaleAnimation CreateScaleAnimation(GameObject animatedObject, float sizeBeginning, float sizeEnd, float duration)
    {
        ScaleAnimation animation = animatedObject.AddComponent<ScaleAnimation>();
        animation.Init(sizeBeginning,sizeEnd);
        animation.duration = duration;
        return animation;
    }
}
