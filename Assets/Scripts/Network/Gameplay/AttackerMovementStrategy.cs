using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackerMovementStrategy : PlayerMovementStrategy
{
    [SerializeField]
    private float ACCELERATION = 70;

    protected override void Awake()
    {
        base.Awake();
        maxVelocity = 45;
        AngularSpeed = 800;
    }

    public void ClampPlayerVelocity()
    {
        myRigidbody.velocity *= Mathf.Min(1.0f, maxVelocity / myRigidbody.velocity.magnitude);
    }
    protected override void Move(float dt)
    {
        var lookPos = targetPosition.Value - transform.position;
        lookPos.y = 0;
        var targetRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, dt * AngularSpeed);
        myRigidbody.AddForce(transform.forward * ACCELERATION);

        ClampPlayerVelocity();
    }
}
