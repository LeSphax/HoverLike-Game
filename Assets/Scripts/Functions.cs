using UnityEngine;

class Functions
{

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        LayerMask layerMask = (1 << 10);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            if (hit.collider.tag == Tags.Ground)
            {
                return hit.point;

            }
        return Vector3.zero;
    }
}

