using System.Collections.Generic;
using UnityEngine;

public class DecisionSB : MonoBehaviour, MLAgents.Decision {


    public float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        return new[] { Random.Range(0,1f), Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f) };
    }

    public List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        return new List<float>();
    }
}
