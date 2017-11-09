using PlayerManagement;
using UnityEngine;

//Put on the camera
public class BattleriteCamera : MonoBehaviour
{
    public float YOffset
    {
        get
        {
            return (DownVisionLength + UpVisionLength)/2;
        }
    }

    private float VerticalAngle
    {
        get
        {
            return 90 - Camera.main.transform.eulerAngles.x;
        }
    }

    private float DownVisionLength
    {
        get
        {
            return transform.position.y * Mathf.Tan(Mathf.Deg2Rad * (Camera.main.fieldOfView / 2 + VerticalAngle));
        }
    }

    private float UpVisionLength
    {
        get
        {
            return transform.position.y * Mathf.Tan(Mathf.Deg2Rad * (Camera.main.fieldOfView / 2 - VerticalAngle));
        }
    }

    private static float HorizontalFOV
    {
        get
        {
            var radAngle = Camera.main.fieldOfView * Mathf.Deg2Rad;
            var radHFOV = 2 * Mathf.Atan(Mathf.Tan(radAngle / 2) * Camera.main.aspect);
            var horizontalFOV = Mathf.Rad2Deg * radHFOV;
            return horizontalFOV;
        }
    }

    private float XOffset
    {
        get
        {
            return transform.position.y * Mathf.Tan(Mathf.Deg2Rad * (HorizontalFOV/2));
        }
    }

    public float speed;

    private Vector3 startPosition;
    private Vector3 previousBasePosition;
    private Vector3 targetPosition;

    public float xMinLimit;
    public float xMaxLimit;
    public float yMinLimit;
    public float yMaxLimit;
    public Vector3 offset;

    private Vector3 BasePosition
    {
        get
        {
            if (Players.MyPlayer != null && Players.MyPlayer.controller != null)
                return startPosition + Vector3.forward * Players.MyPlayer.controller.transform.position.z + Vector3.right * Players.MyPlayer.controller.transform.position.x + offset;
            else
                return transform.localPosition;
        }
    }

    private void Awake()
    {
        if (EditorVariables.HeadlessServer)
            Destroy(this);
        startPosition = transform.localPosition;
        previousBasePosition = transform.localPosition;
    }

    public Vector3 ClampPosition(Vector3 initialPosition)
    {
        return new Vector3(
            Mathf.Clamp(initialPosition.x, yMinLimit + UpVisionLength, yMaxLimit - DownVisionLength),
            initialPosition.y,
            Mathf.Clamp(initialPosition.z, xMinLimit + XOffset, xMaxLimit - XOffset));
    }

    private float GetHalfVisionLength(float fov, float angleOffset)
    {
        return transform.position.y * (Mathf.Tan(Mathf.Deg2Rad * (fov / 2 - angleOffset)) + Mathf.Tan(Mathf.Deg2Rad * (Camera.main.fieldOfView / 2 + angleOffset))) /2;
    }

    Vector3 prevPos;
    Vector3 prevMove;

    private void Update()
    {
        //Update target camera position on Update as it depends on events and the result of the previous FixedUpdate
        if (Camera.main != null && Camera.main.enabled)
        {
            Vector3 previousPosition = transform.position;
            Vector2 mouseProportion = GetMouseProportion();
            targetPosition = new Vector3(BasePosition.x + mouseProportion.y * YOffset, BasePosition.y, BasePosition.z - mouseProportion.x * XOffset);
        }
        else
        {
            targetPosition = BasePosition;
        }
        //Debug.Log("Camera : " + (transform.position - prevPos).magnitude * Time.deltaTime + "    " + (transform.position - prevPos));
        Vector3 currentMove = transform.position - prevPos;
        //if ((currentMove - prevMove).magnitude > (currentMove + prevMove).magnitude)
        //{
        //    Debug.LogWarning("Camera changing direction" + currentMove);
        //}
        prevMove = currentMove;
        prevPos = transform.position;
    }

    //Position the camera on FixedUpdate to avoid jittery effect when it is out of sync with the player position updates
    public void PositionCamera()
    {
        if (Camera.main != null && Camera.main.enabled)
        {
            transform.position = transform.position + BasePosition - previousBasePosition;

            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            transform.position = ClampPosition(transform.position);

            previousBasePosition = BasePosition;
        }
    }

    private Vector2 GetMouseProportion()
    {
        Vector3 clampedMousePosition = new Vector3(Mathf.Clamp(Input.mousePosition.x, 0, Screen.width),
            Mathf.Clamp(Input.mousePosition.y, 0, Screen.height));

        float xDistanceToCenter = clampedMousePosition.x - Screen.width / 2.0f;
        float yDistanceToCenter = clampedMousePosition.y - Screen.height / 2.0f;

        float xProportion = xDistanceToCenter / (Screen.width / 2.0f);
        float yProportion = yDistanceToCenter / (Screen.height / 2.0f);
        //yProportion = Mathf.Min(yProportion * 3, 1);
        return new Vector2(xProportion, yProportion);
    }
}
