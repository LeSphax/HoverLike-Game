using PlayerManagement;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementManager))]
public abstract class PlayerMovementStrategy : SlideBall.MonoBehaviour
{
    public abstract float MaxPlayerVelocity
    {
        get;
    }

    public Vector3? targetPosition;
    public Vector3? TargetPosition
    {
        get
        {
            return targetPosition;
        }
        set
        {
            if (targetPosition != null && value == null)
            {
                StopMoving();
            }
            targetPosition = value;
        }
    }

    protected Rigidbody myRigidbody;

    protected const float FRAME_DURATION = 0.02f;
    internal GameObject target;
    internal PlayerMovementManager movementManager;

    protected virtual void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        movementManager = GetComponent<PlayerMovementManager>();
    }

    public void UpdateMovement()
    {
        if (TargetPosition != null && movementManager.controller.playerConnectionId != MyComponents.Players.MyPlayer.id)
        {
            Move();
        }
        OtherMovementEffects();
    }

    protected abstract void Move();
    protected virtual void OtherMovementEffects() { }

    protected abstract void StopMoving();
}
