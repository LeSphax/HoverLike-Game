using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovementStrategy : MonoBehaviour
{
    public Vector3? targetPosition;

    protected CustomRigidbody myRigidbody;

    [SerializeField]
    internal float maxVelocity;
    [SerializeField]
    internal float AngularSpeed;

    protected virtual void Awake()
    {
        myRigidbody = GetComponent<CustomRigidbody>();
    }

    public void UpdateMovement(float dt)
    {
        if (targetPosition != null)
        {
            Move(dt);
        }
    }

    protected abstract void Move(float dt);
}
