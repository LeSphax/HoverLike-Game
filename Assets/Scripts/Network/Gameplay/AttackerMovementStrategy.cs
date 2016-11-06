using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackerMovementStrategy : PlayerMovementStrategy
{
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
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, FRAME_DURATION * ANGULAR_SPEED);
        myRigidbody.AddForce(transform.forward * ACCELERATION * FRAME_DURATION, ForceMode.VelocityChange);

        ClampPlayerVelocity();
    }
}
