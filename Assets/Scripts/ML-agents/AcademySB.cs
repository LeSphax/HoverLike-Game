using MLAgents;
using UnityEngine;

public class AcademySB : Academy
{
    public static event EmptyEventHandler AcademyResetEvent;

    public static float maxX;
    public static float maxZ;
    public static float maxDistance;
    public static bool goals;

    public override void InitializeAcademy()
    {
#if !UNITY_EDITOR
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
#endif
        foreach (Brain brain in GetComponentsInChildren<Brain>())
        {
#if !UNITY_EDITOR
        brain.brainType = BrainType.External;
#else
            //if (brain.brainType == BrainType.External)
            //    brain.brainType = BrainType.Heuristic;
#endif

        }
    }

    public override void AcademyReset()
    {
        if (AcademyResetEvent != null)
            AcademyResetEvent.Invoke();
        maxX = resetParameters["maxX"];
        maxZ = resetParameters["maxZ"];
        goals = resetParameters["goals"] == 1;
        maxDistance = Mathf.Sqrt(Mathf.Pow(maxX*2, 2) + Mathf.Pow(maxZ * 2, 2)) + 5;
    }

    public override void AcademyStep()
    {


    }
}
