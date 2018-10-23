using Ball;
using MLAgents;
using PlayerManagement;
using UnityEngine;

public class AgentSB : Agent
{
    private const int positionNormalization = 100;
    public static int idCount = 0;
    [HideInInspector]
    public int agentId;
    private PlayerController mPlayerController;
    private Rigidbody mRigidbody;
    private PowerBar mPowerBar;
    private readonly Goal[] mGoals = new Goal[2];

    private Vector3 centerOfField;

    private void Awake()
    {
        brain = FindObjectOfType<Brain>();
        mPlayerController = GetComponent<PlayerController>();
        mPlayerController.Reset += ControllerReset;
        mRigidbody = GetComponent<Rigidbody>();
        Goal[] goals = MyComponents.GetComponentsInChildren<Goal>(true);
        foreach (Goal goal in goals)
        {
            mGoals[goal.teamNumber] = goal;
            goal.GoalScored += GoalScored;
        }
        centerOfField = MyComponents.transform.TransformPoint(Vector3.zero);
    }

    public void Start()
    {
        mPlayerController.abilitiesManager.PlayerHasShot += PlayerHasShot;
    }

    public void ControllerReset()
    {
        mPowerBar = MyComponents.GetComponentInChildren<PowerBar>(true);
        resetPlayer = true;
        AgentReset();
    }

    public override void InitializeAgent()
    {
        agentId = idCount++;
        AcademySB.AcademyResetEvent += AcademyReset;
    }

    private void AcademyReset()
    {
        if (AcademySB.mode == AcademySB.Mode.SHOOT_GOALS)
        {
            agentParameters.maxStep = 500;
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(new Vector2(transform.localPosition.x, transform.localPosition.z) / positionNormalization);
        AddVectorObs(transform.localRotation.eulerAngles.y / 360);
        AddVectorObs(new Vector2(mRigidbody.velocity.x, mRigidbody.velocity.z) / positionNormalization);
        AddVectorObs(new Vector2(MyComponents.BallState.transform.localPosition.x, MyComponents.BallState.transform.localPosition.z) / positionNormalization);
        AddVectorObs(new Vector2(MyComponents.BallState.Rigidbody.velocity.x, MyComponents.BallState.Rigidbody.velocity.x) / positionNormalization);
        foreach (Goal goal in mGoals)
        {
            AddVectorObs(new Vector2(goal.transform.localPosition.x, goal.transform.localPosition.z) / positionNormalization);
            AddVectorObs(goal.transform.localRotation.eulerAngles.y / 360);
        }
        AddVectorObs(MyComponents.Players.MyPlayer.HasBall);
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

        if (AcademySB.mode != AcademySB.Mode.PICK_UP) {
            if (vectorAction[4] >= threshold)
            MyComponents.InputManager.SetMouseButtonDown(0);
            if (vectorAction[5] >= threshold)
                MyComponents.InputManager.SetMouseButtonUp(0);
        }
        MyComponents.InputManager.SetMouseLocalPosition(new Vector3(vectorAction[6], 0, vectorAction[7]) * positionNormalization);

        switch (AcademySB.mode)
        {
            case AcademySB.Mode.PICK_UP:
                RewardBallCatches();
                break;
            case AcademySB.Mode.SHOOT:
                RewardShots();
                break;
            case AcademySB.Mode.SHOOT_GOALS:
                RewardGoals();
                break;
            default:
                Debug.LogError("Mode doesn't exist " + AcademySB.mode);
                break;
        }

        if (agentId == 0 && Time.timeScale < 5f)
        {
            Monitor.SetActive(true);
            Monitor.Log("Reward", GetReward());
            Monitor.Log("CumulativeReward", GetCumulativeReward());
        }
#if UNITY_EDITOR
        if (agentId == 0)
        {
            Debug.Log(GetReward());
        }
#endif
    }

    private void RewardBallCatches()
    {
        if (MyComponents.Players.MyPlayer.HasBall)
        {
            SetReward(1f);
            AgentReset();
        }
        else
        {
            SetReward(-0.001f);
        }
    }

    private float playerHasShot = -1;

    private void PlayerHasShot(float power)
    {
        playerHasShot = power;
    }

    private void RewardShots()
    {
        if (playerHasShot > 0f)
        {
            SetReward(playerHasShot);
            AgentReset();
        }
        else
        {
            SetReward(-0.001f);
        }
    }

    private Team goalScoredAgainstTeam;

    private void GoalScored(Team team)
    {
        goalScoredAgainstTeam = team;
    }

    private void RewardGoals()
    {
        if (goalScoredAgainstTeam != Team.NONE)
        {
            SetReward(goalScoredAgainstTeam != mPlayerController.Player.Team ? 1 : 0);
            Done();
        }
        else
        {
            SetReward(-0.001f);
        }
    }

    private bool resetPlayer = false;

    public override void AgentReset()
    {
        playerHasShot = -1;
        goalScoredAgainstTeam = Team.NONE;

        float maxX = AcademySB.maxX;
        float maxZ = AcademySB.maxZ;

        float[] distanceFromGoals = null;

        if (resetPlayer)
        {
            Vector3? playerPosition = null;

            while (playerPosition == null || AcademySB.HasGoals && (distanceFromGoals[0] < 30 || distanceFromGoals[1] < 30))
            {
                playerPosition = new Vector3(Random.Range(-maxX, maxX), transform.localPosition.y, Random.Range(-maxZ, maxZ));
                distanceFromGoals = new float[] { Vector3.Distance(mGoals[0].transform.localPosition, playerPosition.Value), Vector3.Distance(mGoals[1].transform.localPosition, playerPosition.Value) };
            }
            transform.localPosition = playerPosition.Value;
            mRigidbody.velocity = Vector3.zero;
            resetPlayer = false;
        }

        MyComponents.BallState.trajectoryStrategy = new FreeTrajectoryStrategy(MyComponents.BallState);
        Vector3? ballPosition = null;
        float distanceFromPlayer = 0;

        while (ballPosition == null || distanceFromPlayer < 7 || (AcademySB.HasGoals && (distanceFromGoals[0] < 30 || distanceFromGoals[1] < 30)))
        {
            ballPosition = new Vector3(Random.Range(-maxX, maxX), MyComponents.BallState.transform.localPosition.y, Random.Range(-maxZ, maxZ));
            distanceFromGoals = new float[] { Vector3.Distance(mGoals[0].transform.localPosition, ballPosition.Value), Vector3.Distance(mGoals[1].transform.localPosition, ballPosition.Value) };
            distanceFromPlayer = Vector3.Distance(transform.localPosition, ballPosition.Value);
        }
        MyComponents.BallState.PutBallAtPosition(ballPosition.Value);
    }
}
