using UnityEngine;

[RequireComponent(typeof(PlayerMovementManager))]
public abstract class PlayerMovementStrategy : MonoBehaviour
{
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
        if (TargetPosition != null)
        {
            Move();
        }
    }

    protected abstract void Move();
    protected abstract void StopMoving();
}
