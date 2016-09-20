using UnityEngine;


public class GameController : MonoBehaviour
{

    public float scrollingSpeed;
    private Vector3 cameraPosition;
    private Vector3 initialPosition;
    private const float MAX_CAMERA_OFFSET_X = 60;
    private const float MAX_CAMERA_OFFSET_Z = 15;


    void Start()
    {
        initialPosition = Camera.main.transform.localPosition;
        cameraPosition = Vector3.zero;
        Cursor.lockState = CursorLockMode.Confined;
    }


    void Update()
    {
        if (Input.mousePosition.x < 5 && cameraPosition.x < MAX_CAMERA_OFFSET_X)
        {
            cameraPosition += Vector3.right * scrollingSpeed;
        }
        else if (Input.mousePosition.x > Screen.width - 5 && cameraPosition.x > -MAX_CAMERA_OFFSET_X)
        {
            cameraPosition += Vector3.left * scrollingSpeed;
        }
        else if (Input.mousePosition.y < 5 && cameraPosition.z < MAX_CAMERA_OFFSET_Z)
        {
            cameraPosition += Vector3.forward * scrollingSpeed;
        }
        else if (Input.mousePosition.y > Screen.height - 5 && cameraPosition.z > -MAX_CAMERA_OFFSET_Z)
        {
            cameraPosition += Vector3.back * scrollingSpeed;
        }
        Vector3 newPosition = initialPosition + Camera.main.transform.InverseTransformVector(cameraPosition);
        Camera.main.transform.localPosition = newPosition;
        Cursor.lockState = CursorLockMode.Confined;

    }

}
