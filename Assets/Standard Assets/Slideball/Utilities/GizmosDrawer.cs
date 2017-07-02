
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GizmosDrawer : MonoBehaviour
{
#if UNITY_EDTOR
    private static Dictionary<Vector3, Vector3> lines = new Dictionary<Vector3, Vector3>();
    private static List<Ray> rays = new List<Ray>();

    public static void DrawLine(Vector3 from, Vector3 to)
    {
        lines.Clear();
        lines.Add(from, to);
    }

    public static void DrawRay(Ray ray)
    {
        rays.Clear();
        rays.Add(ray);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var pair in lines)
        {
            Gizmos.DrawLine(pair.Key, pair.Value);
        }
        Gizmos.color = Color.blue;
        foreach (var ray in rays)
            Gizmos.DrawRay(ray);
    }
#endif
}
