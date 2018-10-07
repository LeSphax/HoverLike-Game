using UnityEngine;
using UnityEngine.UI;

public class BillBoard : SlideBall.MonoBehaviour
{
    private Camera m_Camera;

    [SerializeField]
    private Text text;

    public Color Color
    {
        set
        {
            text.color = value;
        }
    }

    public string Text
    {

        set
        {
            text.text = value;
        }
    }

    void Start()
    {
        m_Camera = Camera.main;
    }

    public void SetHeight(float height)
    {
        transform.localPosition = transform.localPosition - Vector3.up * transform.localPosition.y + height * Vector3.up;
    }

    void Update()
    {
        Transform target;
        if (m_Camera.isActiveAndEnabled)
        {
            target = m_Camera.transform;
        }
        else
        {
            target = MyComponents.VictoryPose.victoryCamera.transform;
        }
        transform.LookAt(transform.position + target.rotation * Vector3.forward,
            target.rotation * Vector3.up);
    }
}