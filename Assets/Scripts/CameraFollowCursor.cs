using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowCursor : MonoBehaviour
{

    public RectTransform cursor;
    public float speed;

    private Vector3 startPosition;
    private Vector3 cursorWorldPosition;

    private void Awake()
    {
        startPosition = transform.position;
        Debug.LogWarning(startPosition);
    }

    private void Update()
    {
        cursorWorldPosition = cursorWorldPosition -speed * Vector3.forward * (Input.GetAxis("Mouse X")) + speed * Vector3.right * (Input.GetAxis("Mouse Y"));
        cursor.position = Camera.main.WorldToScreenPoint(cursorWorldPosition);

        float step = Time.deltaTime * speed;

        Vector3 targetPosition = startPosition + cursorWorldPosition - Vector3.up * cursorWorldPosition.y;
        transform.position = Vector3.Lerp(transform.position, targetPosition, step);
    }
}
