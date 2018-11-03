using UnityEngine;

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

        if (MyComponents.MyPlayer.InputManager.GetMouseButtonDown(0))
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
            endPoint.transform.position = MyComponents.MyPlayer.InputManager.GetMouseLocalPosition();
            controlPoint = Instantiate(pointPrefab);
            controlPoint.transform.position = MyComponents.MyPlayer.InputManager.GetMouseLocalPosition();
            bezier.controlPoints[2] = endPoint.transform.position;
        }
        if (MyComponents.MyPlayer.InputManager.GetMouseButton(0))
        {
            controlPoint.transform.position = MyComponents.MyPlayer.InputManager.GetMouseLocalPosition();
            bezier.controlPoints[1] = controlPoint.transform.position;

        }
    }
}