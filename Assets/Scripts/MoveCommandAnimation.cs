using UnityEngine;
using CustomAnimations;

public class MoveCommandAnimation : MyAnimation
{

    public GameObject[] Arrows;
    private const float targetZRotation = -180;

    protected void Start()
    {
        StartAnimating();
    }

    protected override void Animate(float completion)
    {
        foreach (GameObject arrow in Arrows)
        {
            arrow.transform.localRotation = Quaternion.Euler(Vector3.forward * completion * targetZRotation);
        }
    }

    protected override void FinishAnimation()
    {
        base.FinishAnimation();
        Destroy(gameObject);
    }

}
