using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackerMovementStrategy : PlayerMovementStrategy
{
    private float BRAKE_AMOUNT;
    private bool braking;
    private const float JUMP_FORCE = 60f;

    [SerializeField]
    private float ACCELERATION;
    public static float Acceleration = 100;

    [SerializeField]
    private float MAX_VELOCITY;
    public static float MaxVelocity = 75;

    [SerializeField]
    private float ANGULAR_SPEED;
    public static float AngularSpeed = 500;


    public override float MaxPlayerVelocity
    {
        get
        {
            return MAX_VELOCITY;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        BRAKE_AMOUNT = ACCELERATION * 4 / 7;
        ACCELERATION = Acceleration;
        MAX_VELOCITY = MaxVelocity;
        ANGULAR_SPEED = AngularSpeed;
    }

    public void ClampPlayerVelocity()
    {
        myRigidbody.velocity *= Mathf.Min(1.0f, MAX_VELOCITY / myRigidbody.velocity.magnitude);
    }

    protected override void Move()
    {
        var lookPos = TargetPosition.Value - transform.position;
        lookPos.y = 0;
        var targetRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.fixedDeltaTime * ANGULAR_SPEED);
        myRigidbody.AddForce(transform.forward * ACCELERATION, ForceMode.Acceleration);

        ClampPlayerVelocity();
    }

    protected override void OtherMovementEffects()
    {
        Vector3 currentVelocity = myRigidbody.velocity;
        if (braking && currentVelocity.magnitude != 0)
        {
            myRigidbody.velocity -= BRAKE_AMOUNT * Time.fixedDeltaTime * Vector3.Normalize(currentVelocity);
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
