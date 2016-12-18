using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackerMovementStrategy : PlayerMovementStrategy
{
    private const float BRAKE_AMOUNT = 50f;
    private const float JUMP_FORCE = 60f;

    [SerializeField]
    private float ACCELERATION = 70;

    [SerializeField]
    private float MAX_VELOCITY = 45;
    [SerializeField]
    private float ANGULAR_SPEED = 800;

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
        Vector3 currentVelocity = myRigidbody.velocity;
        if (currentVelocity.magnitude != 0)
        {
            myRigidbody.velocity -= BRAKE_AMOUNT * Time.fixedDeltaTime * Vector3.Normalize(currentVelocity);
            if (Vector3.Normalize(currentVelocity) != Vector3.Normalize(myRigidbody.velocity))
            {
                myRigidbody.velocity = Vector3.zero;
                movementManager.controller.DestroyTarget();
            }
        }
    }

    internal void Jump()
    {
        myRigidbody.AddForce(Vector3.up * JUMP_FORCE, ForceMode.VelocityChange);
    }
}
