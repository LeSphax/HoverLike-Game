using Ball;
using MLAgents;
using PlayerManagement;
using UnityEngine;

public class AgentSB : Agent
{
    public static int idCount = 0;
    [HideInInspector]
    public int agentId;

    private void Awake()
    {
        brain = FindObjectOfType<Brain>();
    }

    public override void InitializeAgent()
    {
        agentId = idCount++;
    }

    public override void CollectObservations()
    {
        AddVectorObs(new Vector2(transform.localPosition.x, transform.localPosition.z));
        AddVectorObs(new Vector2(MyComponents.BallState.transform.localPosition.x, MyComponents.BallState.transform.localPosition.z));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        
        float threshold = 0.5f;

        KeyCode[] movementKeys = UserSettings.MovementKeys;
        for (int i = 0; i <= 3; i++)
        {
            if (vectorAction[i] >= threshold)
                MyComponents.InputManager.SetKey(movementKeys[i]);
        }

        if (MyComponents.Players.MyPlayer.HasBall)
        {
            SetReward(1f);
            Done();
        } else
        {
            SetReward(-0.001f);
        }

        if (agentId == 1 && Time.timeScale < 5f)
        {
            Monitor.SetActive(true);
            Monitor.Log("Reward", GetReward());
            Monitor.Log("CumulativeReward", GetCumulativeReward());
        }

    }

    public override void AgentReset()
    {
        transform.localPosition = new Vector3(Random.Range(-10, 10), transform.localPosition.y, Random.Range(-10, 10));
        MyComponents.BallState.trajectoryStrategy = new FreeTrajectoryStrategy(MyComponents.BallState);
        Vector3? ballPosition = null;
        while(ballPosition == null || Vector3.Distance(transform.localPosition, ballPosition.Value) < 5)
        {
            ballPosition = new Vector3(Random.Range(-10, 10), MyComponents.BallState.transform.localPosition.y, Random.Range(-10, 10));
        }
        MyComponents.BallState.PutBallAtPosition(ballPosition.Value);
    }
}
