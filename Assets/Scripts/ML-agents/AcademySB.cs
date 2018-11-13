using MLAgents;
using System.Text;
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
    public static bool randomGoals;
    public static float goalSize;
    public static bool TwoPlayers;

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
        maxX = resetParameters["maxX"];
        maxZ = resetParameters["maxZ"];
        mode = (Mode)resetParameters["mode"];
        randomGoals = resetParameters["randomGoals"] == 1;
        goalSize = resetParameters["goalSize"];
        TwoPlayers = resetParameters["twoPlayers"] == 1;

        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Academy reset " + mode);
        builder.AppendLine("maxX " + maxX);
        builder.AppendLine("maxZ " + maxZ);
        builder.AppendLine("randomGoals " + randomGoals);
        builder.AppendLine("goalSize " + goalSize);
        builder.AppendLine("TwoPlayers " + TwoPlayers);
        Debug.Log(builder.ToString());
        if (AcademyResetEvent != null)
            AcademyResetEvent.Invoke();
    }

    public override void AcademyStep()
    {
        //Debug.Log("AcademyStep " + GetTotalStepCount());

    }
}
