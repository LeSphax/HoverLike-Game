using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BezierMaths{

    public static Vector3 Bezier3(Vector3[] controlPoints, float t)
    {
        return Bezier3(controlPoints[0], controlPoints[1], controlPoints[2], t);
    }

    public static Vector3 Bezier3(Vector3 Start, Vector3 Control, Vector3 End, float t)
    {
        return (((1 - t) * (1 - t)) * Start) + (2 * t * (1 - t) * Control) + ((t * t) * End);
    }

    public static float LengthBezier3(Vector3[] controlPoints, int precision)
    {
        Vector3 previous = Bezier3(controlPoints, 0);
        float length = 0;
        Vector3 current;
        for (int i = 1; i <= precision; i++)
        {
            current = Bezier3(controlPoints, i * (float)1 / precision);
            length += Vector3.Distance(current, previous);
            previous = current;
        }
        return length;
    }
}
