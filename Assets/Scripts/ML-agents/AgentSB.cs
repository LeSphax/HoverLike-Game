using Ball;
using MLAgents;
using PlayerManagement;
using System;
using System.Text;
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
        //if (AcademySB.mode == AcademySB.Mode.SHOOT_GOALS)
        //{
        //    agentParameters.maxStep = 500;
        //}
    }

    private int RaycastAtAngle(float angle, out float distance)
    {
        Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, 1 << Layers.GoalBoundaries | 1 << Layers.Walls, QueryTriggerInteraction.Ignore))
        {
            distance = hit.distance;
            if (hit.collider.gameObject.layer == Layers.Walls)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            Debug.LogError("Should always hit something");
            distance = 0;
            return 0;
        }
    }

    public override void CollectObservations()
    {
        Vector2 playerPosition = new Vector2(transform.localPosition.x, transform.localPosition.z) / positionNormalization;

        AddVectorObs(playerPosition);
        AddVectorObs(new Vector2(Mathf.Cos(transform.localRotation.eulerAngles.y), Mathf.Sin(transform.localRotation.eulerAngles.y)));
        AddVectorObs(new Vector2(mRigidbody.velocity.x, mRigidbody.velocity.z) / positionNormalization);

        Vector2 ballPosition = new Vector2(MyComponents.BallState.transform.localPosition.x, MyComponents.BallState.transform.localPosition.z) / positionNormalization;
        AddVectorObs(ballPosition - playerPosition);
        AddVectorObs(new Vector2(MyComponents.BallState.Rigidbody.velocity.x, MyComponents.BallState.Rigidbody.velocity.x) / positionNormalization);
        foreach (Goal goal in mGoals)
        {
            Vector2 goalPosition = new Vector2(goal.transform.localPosition.x, goal.transform.localPosition.z) / positionNormalization;
            AddVectorObs(goalPosition - playerPosition);
            AddVectorObs(new Vector2(Mathf.Cos(goal.transform.localRotation.eulerAngles.y), Mathf.Sin(goal.transform.localRotation.eulerAngles.y)));
        }
        AddVectorObs(MyComponents.MyPlayer.HasBall);
        AddVectorObs(mPowerBar.IsFilling());
        AddVectorObs(mPowerBar.powerValue);
        AddVectorObs(AcademySB.episodeCompletion);
        int numberOfRaycasts = 16;
        for (int i = 0; i < numberOfRaycasts; i++)
        {
            float angle = (Mathf.PI * 2 / numberOfRaycasts) * i;
            float distance;
            int type = RaycastAtAngle(angle, out distance);
            AddVectorObs(type);
            AddVectorObs(distance / positionNormalization);
        }
    }

    private StringBuilder LogStuff()
    {
        StringBuilder builder = new StringBuilder();
        if (AcademySB.mode == AcademySB.Mode.PICK_UP)
            builder.AppendLine("HasBall " + MyComponents.MyPlayer.HasBall);
        if (AcademySB.mode == AcademySB.Mode.SHOOT)
            builder.AppendLine("PlayerHasShot " + playerHasShot);
        if (AcademySB.mode == AcademySB.Mode.SHOOT_GOALS)
            builder.AppendLine("Goal scored against team " + goalScoredAgainstTeam);
        return builder;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        StringBuilder builder = null;
        if (agentId == 0)
        {
            builder = LogStuff();
        }
        // Actions
        // 4 Movement keys
        // 1 Load key
        // 1 Shoot key
        // cos and sin of player rotation
        // distance of target
        float threshold = 0.5f;

        KeyCode[] movementKeys = UserSettings.MovementKeys;
        for (int i = 0; i <= 3; i++)
        {
            if (vectorAction[i] >= threshold)
                mPlayerController.inputManager.SetKey(movementKeys[i]);
        }

        if (AcademySB.mode != AcademySB.Mode.PICK_UP)
        {
            if (vectorAction[4] >= threshold)
                mPlayerController.inputManager.SetMouseButtonDown(0);
            if (vectorAction[5] >= threshold)
                mPlayerController.inputManager.SetMouseButtonUp(0);
        }
        float distance = vectorAction[8] * positionNormalization;
        mPlayerController.inputManager.SetMouseLocalPosition(new Vector3(transform.localPosition.x, 0, transform.localPosition.z) + new Vector3(vectorAction[6], 0, vectorAction[7]) * distance);

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

        if (agentId == 0)
        {
            builder.AppendLine("Reward " + GetReward());
            if (Time.timeScale < 5f)
            {
                Monitor.SetActive(true);
                Monitor.Log("Reward", GetReward());
                Monitor.Log("CumulativeReward", GetCumulativeReward());
            }
            //Debug.Log(builder.ToString());
        }
    }

    private void RewardBallCatches()
    {
        if (MyComponents.MyPlayer.HasBall)
        {
            SetReward(1f);
            AgentReset();
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
            SetReward(goalScoredAgainstTeam != mPlayerController.Player.Team ? 1 : 1);
            AgentReset();
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
                playerPosition = new Vector3(UnityEngine.Random.Range(-maxX, maxX), transform.localPosition.y, UnityEngine.Random.Range(-maxZ, maxZ));
                distanceFromGoals = new float[] { Vector3.Distance(mGoals[0].transform.localPosition, playerPosition.Value), Vector3.Distance(mGoals[1].transform.localPosition, playerPosition.Value) };
            }
            transform.localPosition = playerPosition.Value;
            mRigidbody.velocity = Vector3.zero;
            resetPlayer = false;
        }

        MyComponents.BallState.trajectoryStrategy = new FreeTrajectoryStrategy(MyComponents.BallState);
        Vector3? ballPosition = null;
        float distanceFromPlayer = 0;

        while (ballPosition == null || distanceFromPlayer < 7)
        {
            float ballMaxX = AcademySB.HasGoals ? maxX - 30 : maxX;
            float ballMaxZ = AcademySB.HasGoals ? maxZ - 30 : maxZ;
            ballPosition = new Vector3(UnityEngine.Random.Range(-ballMaxX, ballMaxX), MyComponents.BallState.transform.localPosition.y, UnityEngine.Random.Range(-ballMaxZ, ballMaxZ));
            distanceFromGoals = new float[] { Vector3.Distance(mGoals[0].transform.localPosition, ballPosition.Value), Vector3.Distance(mGoals[1].transform.localPosition, ballPosition.Value) };
            distanceFromPlayer = Vector3.Distance(transform.localPosition, ballPosition.Value);
        }
        MyComponents.BallState.PutBallAtPosition(ballPosition.Value);
    }
}
