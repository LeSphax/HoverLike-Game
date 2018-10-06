using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionSB : MonoBehaviour, MLAgents.Decision {


    public float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        return new[] { 0.0f, 0.0f, 0.0f, 1.0f };
    }

    public List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        return new List<float>();
    }
}
