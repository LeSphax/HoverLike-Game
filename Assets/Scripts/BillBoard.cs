using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BillBoard : MonoBehaviour
{
    private Camera m_Camera;

    void Start()
    {
        m_Camera = Camera.main;
    }

    void Update()
    {
        Transform target;
        if (m_Camera.isActiveAndEnabled)
        {
            target = m_Camera.transform;
            transform.localPosition = transform.localPosition - Vector3.up * transform.localPosition.y + 8 * Vector3.up;
        }
        else
        {
            target = MyComponents.VictoryPose.victoryCamera.transform;
            transform.localPosition = transform.localPosition - Vector3.up * transform.localPosition.y + 6 * Vector3.up;
        }
        transform.LookAt(transform.position + target.rotation * Vector3.forward,
            target.rotation * Vector3.up);
    }
}