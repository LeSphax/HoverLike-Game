using UnityEngine;


public class GameController : MonoBehaviour
{

    public float scrollingSpeed;
    private Vector3 cameraPosition;
    private Vector3 initialPosition;
    private const float MAX_CAMERA_OFFSET_X = 120;
    private const float MAX_CAMERA_OFFSET_Z = 30;

    [SerializeField]
    private Texture2D cursor;

    private bool activated = false;

    void Start()
    {
        initialPosition = Camera.main.transform.localPosition;
        cameraPosition = Vector3.zero;
        Cursor.lockState = CursorLockMode.Confined;
      //  Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    }


    void Update()
    {
        if (activated)
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

        }
       // DesactivateOnXPressed();
        if (Input.GetKeyDown(KeyCode.F4))
        {
            Cursor.lockState = CursorLockMode.Confined;
            Screen.fullScreen = true;
        }
    }

    void DesactivateOnXPressed()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            activated = !activated;
        }
    }

}
