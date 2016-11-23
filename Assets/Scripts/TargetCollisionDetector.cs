using UnityEngine;

public class TargetCollisionDetector : MonoBehaviour
{
    [HideInInspector]
    public PlayerController controller;

    void OnTriggerEnter(Collider other)
    {
        if (IsObjectInMyPlayer(other.transform))
        {
            if (IsObjectInMesh(other.transform))
            {
                controller.TargetHit();
            }
        }
    }

    private bool IsObjectInMyPlayer(Transform t)
    {
        if (t.tag == Tags.MyPlayer || t.tag == Tags.Player)
        {
            return t.GetComponent<PlayerController>() == controller;
        }
        else if (t.parent == null)
        {
            return false;
        }
        else
        {
            return IsObjectInMyPlayer(t.parent);
        }
    }

    private bool IsObjectInMesh(Transform t)
    {
        if (t.tag == Tags.Mesh)
        {
            return true;
        }
        else if (t.parent != null)
        {
            return IsObjectInMesh(t.parent);
        }
        else
        {
            return false;
        }
    }
}
