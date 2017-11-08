using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private Vector3? TargetPosition
    {
        get
        {
            if (controller.movementManager != null)
                return controller.movementManager.TargetPosition;
            return null;
        }
        set
        {
            if (controller.movementManager != null)
            {
                if (value == null)
                {
                    controller.movementManager.TargetPosition = null;
                }
                else
                {
                    controller.movementManager.TargetPosition = new Vector3(value.Value.x, 0, value.Value.y);
                }
            }
        }
    }
    private PlayerController controller;

    float slope;
    float constant;
    private const float DISTANCE_THRESHOLD = 0.5f;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    public void SetTarget(Vector2 newPosition)
    {
        TargetPosition = newPosition;
        //
        float normalSlope = (transform.position.z - newPosition.y) / (transform.position.x - newPosition.x);
        slope = -(1 / normalSlope);
        constant = -slope * newPosition.x + newPosition.y;
    }

    private void FixedUpdate()
    {
        if (TargetPosition != null)
        {
            if (Functions.DistanceFromLine(slope, constant, transform.position.x, transform.position.z) < DISTANCE_THRESHOLD)
            {
                CancelTarget();
            }
        }
    }

    internal void CancelTarget()
    {
        TargetPosition = null;
    }
}
