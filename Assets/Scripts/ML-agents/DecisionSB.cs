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
            var mousePosition = Functions.GetMouseWorldPosition();
            actions[6] = mousePosition.x / 100;
            actions[7] = mousePosition.z / 100;
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
