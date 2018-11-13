using PlayerManagement;
using UnityEngine;

//Put on the camera
public class BattleriteCamera : SlideBall.MonoBehaviour
{
    public float YOffset
    {
        get
        {
            return (DownVisionLength + UpVisionLength) / 2;
        }
    }

    private float VerticalAngle
    {
        get
        {
            return 90 - mCamera.transform.eulerAngles.x;
        }
    }

    private float DownVisionLength
    {
        get
        {
            return transform.position.y * Mathf.Tan(Mathf.Deg2Rad * (mCamera.fieldOfView / 2 + VerticalAngle));
        }
    }

    private float UpVisionLength
    {
        get
        {
            return transform.position.y * Mathf.Tan(Mathf.Deg2Rad * (mCamera.fieldOfView / 2 - VerticalAngle));
        }
    }

    private float HorizontalFOV
    {
        get
        {
            var radAngle = mCamera.fieldOfView * Mathf.Deg2Rad;
            var radHFOV = 2 * Mathf.Atan(Mathf.Tan(radAngle / 2) * mCamera.aspect);
            var horizontalFOV = Mathf.Rad2Deg * radHFOV;
            return horizontalFOV;
        }
    }

    private float XOffset
    {
        get
        {
            return transform.position.y * Mathf.Tan(Mathf.Deg2Rad * (HorizontalFOV / 2));
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

    private Camera mCamera;

    private Vector3 BasePosition
    {
        get
        {
            Player myPlayer = MyComponents.MyPlayer;
            if (myPlayer != null && myPlayer.controller != null)
                return new Vector3(
                    startPosition.x + offset.x + myPlayer.controller.transform.position.x,
                    startPosition.y + offset.y,
                    startPosition.z + offset.z + myPlayer.controller.transform.position.z
                );
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

    private void Start(){
        mCamera = Camera.main;
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
        return transform.position.y * (Mathf.Tan(Mathf.Deg2Rad * (fov / 2 - angleOffset)) + Mathf.Tan(Mathf.Deg2Rad * (mCamera.fieldOfView / 2 + angleOffset))) / 2;
    }

    private void Update()
    {
        //Update target camera position on Update as it depends on events and the result of the previous FixedUpdate
        if (mCamera != null && mCamera.enabled)
        {
            Vector3 previousPosition = transform.position;
            Vector2 mouseProportion = GetMouseProportion();
            targetPosition = new Vector3(BasePosition.x + mouseProportion.y * YOffset, BasePosition.y, BasePosition.z - mouseProportion.x * XOffset);
        }
        else
        {
            targetPosition = BasePosition;
        }
    }

    //Position the camera on FixedUpdate to avoid jittery effect when it is out of sync with the player position updates
    public void PositionCamera()
    {
        if (mCamera != null && mCamera.enabled)
        {
            transform.position = transform.position + BasePosition - previousBasePosition;

            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            //transform.position = ClampPosition(transform.position);

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
