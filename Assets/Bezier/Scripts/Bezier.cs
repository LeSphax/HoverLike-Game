using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class Bezier : MonoBehaviour
{
    public Vector3?[] controlPoints = new Vector3?[3];
    public LineRenderer lineRenderer;

    private int layerOrder = 0;
    private int SEGMENT_COUNT = 50;

    private bool activated;
    public bool Activated
    {
        get
        {
            return activated;
        }
        set
        {
            lineRenderer.enabled = value;
            activated = value;
        }
    }

    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.sortingLayerID = layerOrder;
    }

    void Update()
    {
        DrawCurve();
    }

    void DrawCurve()
    {
        if (activated && controlPoints.Where(point => point == null).Count() == 0)
            for (int i = 1; i <= SEGMENT_COUNT; i++)
            {
                float t = i / (float)SEGMENT_COUNT;
                int nodeIndex = 0;
                Vector3 pixel = Functions.Bezier3(controlPoints[nodeIndex].Value,
                    controlPoints[nodeIndex + 1].Value, controlPoints[nodeIndex + 2].Value, t);
                lineRenderer.positionCount = i;
                lineRenderer.SetPosition((i - 1), pixel);
            }
    }
}
