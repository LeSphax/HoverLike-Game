using MLAgents;
using UnityEngine;

public class AcademySB : Academy
{
    public static event EmptyEventHandler AcademyResetEvent;
    private static AcademySB instance;

    public enum Mode
    {
        PICK_UP,
        SHOOT,
        SHOOT_GOALS
    }

    public static float episodeCompletion
    {
        get
        {
            return (float)instance.GetStepCount() / instance.maxSteps;
        }
    }

    public static bool HasGoals
    {
        get
        {
            return mode == Mode.SHOOT_GOALS;
        }
    }

    public static Mode mode;

    public static float maxX;
    public static float maxZ;
    public static float maxDistance;
    public static bool rewardLook;

    public override void InitializeAcademy()
    {
        instance = this;
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
        mode = (Mode)resetParameters["mode"];
        rewardLook = resetParameters["rewardLook"] == 1;
        maxDistance = Mathf.Sqrt(Mathf.Pow(maxX * 2, 2) + Mathf.Pow(maxZ * 2, 2)) + 5;
    }

    public override void AcademyStep()
    {


    }
}
