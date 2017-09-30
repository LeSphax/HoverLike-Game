using UnityEngine;

public class AttackerMovementStrategy : PlayerMovementStrategy
{
    private bool braking;
    private const float JUMP_FORCE = 60f;

    public static float Acceleration = 100;

    public static float MaxVelocity = 75;

    public static float AngularSpeed = 500;

    public static float BrakeProportion = 5.5f/7;

    private float BrakeAmount
    {
        get
        {
            return BrakeProportion * Acceleration;
        }
    }



    public override float MaxPlayerVelocity
    {
        get
        {
            return MaxVelocity;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void ClampPlayerVelocity()
    {
        myRigidbody.velocity *= Mathf.Min(1.0f, MaxVelocity / myRigidbody.velocity.magnitude);
    }

    protected override void Move()
    {
        var lookPos = TargetPosition.Value - transform.position;
        lookPos.y = 0;
        var targetRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.fixedDeltaTime * AngularSpeed);
        //myRigidbody.AddForce(transform.forward * Acceleration, ForceMode.Acceleration);

        //ClampPlayerVelocity();
    }

    protected override void OtherMovementEffects()
    {
        Vector3 currentVelocity = myRigidbody.velocity;
        if (braking && currentVelocity.magnitude != 0)
        {
            myRigidbody.velocity -= BrakeAmount * Time.fixedDeltaTime * Vector3.Normalize(currentVelocity);
        }
    }

    protected override void StopMoving()
    {
    }

    internal void Brake(bool activate)
    {
        //If the player just started to brake, cancel his target.
        if (activate)
        {
            TargetPosition = null;
        }
        braking = activate;
    }

    internal void Jump()
    {
        myRigidbody.AddForce(Vector3.up * JUMP_FORCE, ForceMode.VelocityChange);
    }
}
