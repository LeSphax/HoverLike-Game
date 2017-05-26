using UnityEngine;

public class Zoom : MonoBehaviour
{

    public float speed = 3;
    public float fovMin = 20;
    public float fovMax = 70;

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * speed, fovMin, fovMax);
        }
    }

}