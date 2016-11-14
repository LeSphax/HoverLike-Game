using UnityEngine;

public class GoalieMovementStrategy : PlayerMovementStrategy
{
    private int inZone = 0;

    protected override void Awake()
    {
        base.Awake();
        maxVelocity = 20;
        AngularSpeed = 2000;
    }

    protected override void Move(float dt)
    {
        var lookPos = targetPosition.Value - transform.position;
        lookPos.y = 0;
        var targetRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, dt * AngularSpeed);
        if (Mathf.Approximately(Quaternion.Angle(transform.rotation, targetRotation), 0f))
            //Goalies don't accelerate, they have a constant speed
            myRigidbody.velocity = transform.forward * maxVelocity * (1 + 0.5f * inZone);
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
