using PlayerManagement;
using UnityEngine;

//Put on the camera
public class BattleriteCamera : MonoBehaviour
{
    public float xOffset;
    public float yOffset;

    public float speed;

    private Vector3 startPosition;
    private Vector3 previousBasePosition;

    public float xMinLimit;
    public float xMaxLimit;
    public float yMinLimit;
    public float yMaxLimit;

    private float previousTime;

    private Vector3 basePosition
    {
        get
        {
            if (Players.MyPlayer != null && Players.MyPlayer.controller != null)
                return startPosition + Vector3.forward * Players.MyPlayer.controller.transform.position.z + Vector3.right * Players.MyPlayer.controller.transform.position.x;
            else
                return transform.localPosition;
        }
    }

    private void Awake()
    {
        startPosition = transform.localPosition;
        previousBasePosition = transform.localPosition;
    }

    private void OnEnable()
    {
        LateFixedUpdate.evt += PositionCamera;
    }
    private void OnDisable()
    {
        LateFixedUpdate.evt -= PositionCamera;
    }

    public Vector3 ClampPosition(Vector3 initialPosition)
    {
        return new Vector3(Mathf.Clamp(initialPosition.x, yMinLimit, yMaxLimit), initialPosition.y, Mathf.Clamp(initialPosition.z, xMinLimit, xMaxLimit));
    }

    public void PositionCamera()
    {
        Vector3 previousPosition = transform.position;
        Vector2 mouseProportion = GetMouseProportion();
        Vector3 targetPosition = new Vector3(basePosition.x + mouseProportion.y * yOffset, basePosition.y, basePosition.z - mouseProportion.x * xOffset);


        transform.position = transform.position + basePosition - previousBasePosition;
        float currentTime = TimeManagement.NetworkTimeInSeconds;

        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.fixedDeltaTime);
        previousTime = currentTime;

        transform.position = ClampPosition(transform.position);
        //Debug.Log("Camera base displacement: " + (basePosition - previousBasePosition) + (transform.position - previousPosition));

        previousBasePosition = basePosition;
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
