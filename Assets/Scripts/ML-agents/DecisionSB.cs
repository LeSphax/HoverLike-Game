using System.Collections.Generic;
using UnityEngine;

public class DecisionSB : SlideBall.MonoBehaviour, MLAgents.Decision
{

    BrainSB brain;
    void Awake()
    {
        brain = GetComponent<BrainSB>();
    }


    public float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        List<float> actions = new List<float>();
        for (int i = 0; i < brain.brainParameters.vectorActionSize[0]; i++)
        {
            actions.Add(Random.Range(0, 1f));
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Vector2 goalPosition = new Vector2(vectorObs[10], vectorObs[11]);
            float angle = Mathf.Atan2(goalPosition[1], goalPosition[0]);
            actions[6] = Mathf.Cos(angle);
            actions[7] = Mathf.Sin(angle);
            actions[8] = goalPosition.magnitude;
        }
        if (!Input.GetMouseButtonDown(0))
        {
            actions[4] = 0f;
        }
        if (!Input.GetMouseButtonUp(0))
            actions[5] = 0f;
        return actions.ToArray();
    }

    public List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        return new List<float>();
    }
}
