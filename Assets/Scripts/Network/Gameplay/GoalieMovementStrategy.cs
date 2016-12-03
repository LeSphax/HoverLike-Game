using UnityEngine;

public class GoalieMovementStrategy : PlayerMovementStrategy
{
    [SerializeField]
    private float ANGULAR_SPEED = 2000;
    [SerializeField]
    private float SPEED = 22;

    private int inZone = 0;

    protected override void Move()
    {
        var lookPos = targetPosition.Value - transform.position;
        lookPos.y = 0;
        var targetRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, FRAME_DURATION * ANGULAR_SPEED);
        if (Mathf.Approximately(Quaternion.Angle(transform.rotation, targetRotation), 0f))
            myRigidbody.velocity = transform.forward * SPEED * (1 + 0.4f * inZone);
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
