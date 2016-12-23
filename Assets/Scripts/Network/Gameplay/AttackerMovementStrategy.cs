using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackerMovementStrategy : PlayerMovementStrategy
{
    private float BRAKE_AMOUNT;
    private bool braking;
    private bool hasBraked;
    private const float JUMP_FORCE = 60f;

    [SerializeField]
    private float ACCELERATION = 70;

    [SerializeField]
    private float MAX_VELOCITY = 45;
    [SerializeField]
    private float ANGULAR_SPEED = 500;

    protected override void Awake()
    {
        base.Awake();
        BRAKE_AMOUNT = ACCELERATION * 5 / 7;
    }

    public void ClampPlayerVelocity()
    {
        myRigidbody.velocity *= Mathf.Min(1.0f, MAX_VELOCITY / myRigidbody.velocity.magnitude);
    }

    protected override void Move()
    {
        var lookPos = targetPosition.Value - transform.position;
        lookPos.y = 0;
        var targetRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.fixedDeltaTime * ANGULAR_SPEED);
        myRigidbody.AddForce(transform.forward * ACCELERATION, ForceMode.Acceleration);

        ClampPlayerVelocity();
    }

    internal void Brake()
    {
        //If the player just started to brake, cancel his target.
        if (!braking)
        {
            targetPosition = null;
            braking = true;
        }
        hasBraked = true;
        Vector3 currentVelocity = myRigidbody.velocity;
        if (currentVelocity.magnitude != 0)
        {
            myRigidbody.velocity -= BRAKE_AMOUNT * Time.fixedDeltaTime * Vector3.Normalize(currentVelocity);
            //if (Vector3.Normalize(currentVelocity) != Vector3.Normalize(myRigidbody.velocity))
            //{
            //    myRigidbody.velocity = Vector3.zero;
            //    targetPosition = null;
            //}
        }
    }

    internal void Jump()
    {
        myRigidbody.AddForce(Vector3.up * JUMP_FORCE, ForceMode.VelocityChange);
    }

    private void LateUpdate()
    {
        if (!hasBraked)
            braking = false;
        hasBraked = false;
    }
}
