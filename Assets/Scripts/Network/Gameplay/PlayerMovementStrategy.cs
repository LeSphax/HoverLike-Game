using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMovementStrategy : MonoBehaviour
{
    public Vector3? targetPosition;

    protected CustomRigidbody myRigidbody;

    protected const float FRAME_DURATION = 0.02f;
    internal GameObject target;

    void Awake()
    {
        myRigidbody = GetComponent<CustomRigidbody>();
    }

    public void UpdateMovement()
    {
        if (targetPosition != null)
        {
            Move();
        }
    }

    protected abstract void Move();
}
