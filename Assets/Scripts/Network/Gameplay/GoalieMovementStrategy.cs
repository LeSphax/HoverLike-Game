using System;
using UnityEngine;

public class GoalieMovementStrategy : PlayerMovementStrategy
{
    [SerializeField]
    private float ANGULAR_SPEED = 2000;
    [SerializeField]
    private float SPEED;
    public static float Speed = 30;

    public override float MaxPlayerVelocity
    {
        get
        {
            return SPEED;
        }
    }

    private int inZone = 0;

    protected override void Awake()
    {
        base.Awake();
        SPEED = Speed;
    }

    protected override void Move()
    {
        var lookPos = TargetPosition.Value - transform.position;
        lookPos.y = 0;
        var targetRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, FRAME_DURATION * ANGULAR_SPEED);

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.05f)
        {
            myRigidbody.velocity = transform.forward * SPEED * (1 + 0.3f * inZone);
        }
    }

    protected override void StopMoving()
    {
        myRigidbody.velocity = Vector3.zero;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tags.GoalZone)
        {
            inZone = 1;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == Tags.GoalZone)
        {
            inZone = 0;
        }
    }
}
