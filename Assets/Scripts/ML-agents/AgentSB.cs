using Ball;
using MLAgents;
using UnityEngine;

public class AgentSB : Agent
{
    public static int idCount = 0;
    [HideInInspector]
    public int agentId;
    private Rigidbody mRigidbody;
    private PowerBar mPowerBar;
    private Goal[] mGoals = new Goal[2];

    private void Awake()
    {
        brain = FindObjectOfType<Brain>();
        mRigidbody = GetComponent<Rigidbody>();
        mPowerBar = MyComponents.GetComponentInChildren<PowerBar>();
        Goal[] goals = MyComponents.GetComponentsInChildren<Goal>();
        foreach (Goal goal in goals)
        {
            mGoals[goal.teamNumber] = goal;
        }
    }

    public override void InitializeAgent()
    {
        agentId = idCount++;
        AcademySB.AcademyResetEvent += AcademyReset;
    }

    private void AcademyReset()
    {
        if (AcademySB.maxX < 20)
        {

        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(new Vector2(transform.localPosition.x, transform.localPosition.z) / 100);
        AddVectorObs(transform.localRotation.eulerAngles.y / 360);
        Debug.Log(transform.localRotation.eulerAngles.y);
        AddVectorObs(new Vector2(mRigidbody.velocity.x, mRigidbody.velocity.z) / 100);
        AddVectorObs(new Vector2(MyComponents.BallState.transform.localPosition.x, MyComponents.BallState.transform.localPosition.z) / 100);
        AddVectorObs(new Vector2(MyComponents.BallState.Rigidbody.velocity.x, MyComponents.BallState.Rigidbody.velocity.x) / 100);
        foreach (Goal goal in mGoals)
        {
            AddVectorObs(new Vector2(goal.transform.localPosition.x, goal.transform.localPosition.z) / 100);
            AddVectorObs(goal.transform.localRotation.eulerAngles.y / 360);
        }
        AddVectorObs(mPowerBar.IsFilling());
        AddVectorObs(mPowerBar.powerValue);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Actions
        // 4 Movement keys
        // 1 Load key
        // 1 Shoot key
        // x and z target positions of shot
        float threshold = 0.5f;

        KeyCode[] movementKeys = UserSettings.MovementKeys;
        for (int i = 0; i <= 3; i++)
        {
            if (vectorAction[i] >= threshold)
                MyComponents.InputManager.SetKey(movementKeys[i]);
        }

        if (Vector3.Distance(transform.localPosition, MyComponents.BallState.transform.localPosition) > AcademySB.maxDistance)
        {
            SetReward(-0.3f);
            resetPlayer = true;
            Done();
        }

        if (MyComponents.Players.MyPlayer.HasBall)
        {
            SetReward(1f);
            AgentReset();
        }
        else
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

    private bool resetPlayer = false;

    public override void AgentReset()
    {
        float maxX = AcademySB.maxX;
        float maxZ = AcademySB.maxZ;

        if (resetPlayer)
        {
            transform.localPosition = new Vector3(Random.Range(-maxX, maxX), transform.localPosition.y, Random.Range(-maxZ, maxZ));
            mRigidbody.velocity = Vector3.zero;
            resetPlayer = false;
        }

        foreach (Goal goal in mGoals)
        {
            if (AcademySB.goals)
            {
                goal.gameObject.SetActive(true);
                Vector3? goalPosition = null;
                while (goalPosition == null || Vector3.Distance(transform.localPosition, goalPosition.Value) < 30)
                {
                    goalPosition = new Vector3(Random.Range(-maxX, maxX), goal.transform.localPosition.y, Random.Range(-maxZ, maxZ));
                }
                goal.transform.localPosition = goalPosition.Value;
            }
            else
            {
                goal.gameObject.SetActive(false);
            }
        }

        MyComponents.BallState.trajectoryStrategy = new FreeTrajectoryStrategy(MyComponents.BallState);
        Vector3? ballPosition = null;
        float[] distanceFromGoals = null;
        float distanceFromPlayer = 0;

        while (ballPosition == null || distanceFromPlayer < 7 || (AcademySB.goals && (distanceFromGoals[0] < 30 || distanceFromGoals[1] < 30)))
        {
            ballPosition = new Vector3(Random.Range(-maxX, maxX), MyComponents.BallState.transform.localPosition.y, Random.Range(-maxZ, maxZ));
            distanceFromGoals = new float[] { Vector3.Distance(mGoals[0].transform.localPosition, ballPosition.Value), Vector3.Distance(mGoals[1].transform.localPosition, ballPosition.Value) };
            distanceFromPlayer = Vector3.Distance(transform.localPosition, ballPosition.Value);
        }
        MyComponents.BallState.PutBallAtPosition(ballPosition.Value);
    }
}
