﻿using UnityEngine;

public class MouseInputs : SlideBall.MonoBehaviour {

    public GameObject pointPrefab;
    public Bezier bezier;
    public GameObject startPoint;
    private GameObject controlPoint;
    private GameObject endPoint;

    private void Start()
    {
        bezier.controlPoints[0] = startPoint.transform.position;
    }

    void Update()
    {

        if (MyComponents.InputManager.GetMouseButtonDown(0))
        {
            bezier.Activated=true;
            if (endPoint != null)
            {
                bezier.controlPoints[1] = null;
                bezier.controlPoints[2] = null;
                Destroy(endPoint);
                Destroy(controlPoint);
            }
            endPoint = Instantiate(pointPrefab);
            endPoint.transform.position = MyComponents.InputManager.GetMouseLocalPosition();
            controlPoint = Instantiate(pointPrefab);
            controlPoint.transform.position = MyComponents.InputManager.GetMouseLocalPosition();
            bezier.controlPoints[2] = endPoint.transform.position;
        }
        if (MyComponents.InputManager.GetMouseButton(0))
        {
            controlPoint.transform.position = MyComponents.InputManager.GetMouseLocalPosition();
            bezier.controlPoints[1] = controlPoint.transform.position;

        }
    }
}