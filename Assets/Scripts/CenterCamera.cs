using PlayerManagement;
using UnityEngine;

public class CenterCamera : MonoBehaviour
{

    private Vector3 startPosition;

    public float xMinLimit;
    public float xMaxLimit;
    public float yMinLimit;
    public float yMaxLimit;

    private void Awake()
    {
        startPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (Players.MyPlayer != null)
        {
            GameObject.FindGameObjectWithTag("GameController").transform.position = startPosition + Players.MyPlayer.controller.transform.position;
            transform.position = ClampPosition(transform.position);
        }
    }

    public Vector3 ClampPosition(Vector3 initialPosition)
    {
         return new Vector3(Mathf.Clamp(initialPosition.x, yMinLimit, yMaxLimit), initialPosition.y, Mathf.Clamp(initialPosition.z, xMinLimit, xMaxLimit));
    }
}
