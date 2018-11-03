using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPSLogger: MonoBehaviour
{
    const float fpsMeasurePeriod = 1f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;

    private int fixedUpdate = 0;


    private void Start()
    {
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
    }

    private void FixedUpdate()
    {
        fixedUpdate++;
        Debug.Log(Time.fixedDeltaTime);
    }

    private void Update()
    {
        m_FpsAccumulator++;
        if (Time.realtimeSinceStartup > m_FpsNextPeriod)
        {
            m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
            m_FpsAccumulator = 0;
            m_FpsNextPeriod += fpsMeasurePeriod;
            fixedUpdate = 0;
        }
        
    }
}
