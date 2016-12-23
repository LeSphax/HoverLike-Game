using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private Vector3? TargetPosition
    {
        get
        {
            if (controller.movementManager != null)
                return controller.movementManager.targetPosition;
            return null;
        }
        set
        {
            if (controller.movementManager != null)
            {
                if (value == null)
                {
                    controller.movementManager.targetPosition = null;
                }
                else
                {
                    controller.movementManager.targetPosition = new Vector3(value.Value.x, 0, value.Value.y);
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

    //void OnTriggerEnter(Collider other)
    //{
    //    //if (IsObjectInMyPlayer(other.transform))
    //    //{
    //    if (IsObjectInMesh(other.transform))
    //    {
    //        controller.TargetHit();
    //    }
    //    //}
    //}

    //private bool IsObjectInMyPlayer(Transform t)
    //{
    //    if (t.tag == Tags.MyPlayer || t.tag == Tags.Player)
    //    {
    //        return t.GetComponent<PlayerController>() == controller;
    //    }
    //    else if (t.parent == null)
    //    {
    //        return false;
    //    }
    //    else
    //    {
    //        return IsObjectInMyPlayer(t.parent);
    //    }
    //}

    //private bool IsObjectInMesh(Transform t)
    //{
    //    if (t.tag == Tags.Mesh)
    //    {
    //        return true;
    //    }
    //    else if (t.tag == Tags.Ball)
    //    {
    //        return false;
    //    }
    //    else if (t.parent != null)
    //    {
    //        return IsObjectInMesh(t.parent);
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
}
