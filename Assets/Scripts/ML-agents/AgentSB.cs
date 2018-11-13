using Ball;
using Byn.Net;
using MLAgents;
using PlayerManagement;
using System.Text;
using UnityEngine;

public class AgentSB : Agent
{
    public const int PositionNormalization = 100;
    public static int idCount = 0;
    public int agentId;
    private PlayerMLData mPlayer;

    private PlayerMLData otherPlayer;
    private readonly Goal[] mGoals = new Goal[2];
    private bool ballCaughtOnce = false;

    private void Awake()
    {
        brain = FindObjectOfType<Brain>();
        MyComponents.GameInit.AllObjectsCreated += Init;
        agentId = idCount++;
    }

    public void Init()
    {
        mPlayer = new PlayerMLData(GetComponent<PlayerController>());
        mPlayer.controller.abilitiesManager.PlayerHasShot += PlayerHasShot;
        otherPlayer = new PlayerMLData(MyComponents.Players.GetPlayersInTeam(Teams.GetOtherTeam(mPlayer.controller.Player.team))[0].controller);
        resetPlayer = true;
        Goal[] goals = MyComponents.GetComponentsInChildren<Goal>(true);
        foreach (Goal goal in goals)
        {
            int idx = goal.teamNumber == (int)mPlayer.controller.Player.Team ? 0 : 1;
            mGoals[idx] = goal;
            goal.GoalScored += GoalScored;
        }
        AgentReset();
        AcademySB.AcademyResetEvent += AcademyReset;
        AcademyReset();
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.C)){
            AcademySB.TwoPlayers = false;
            FindObjectOfType<AcademySB>().resetParameters["twoPlayers"] = 0;
            AcademyReset();
        }
        if(Input.GetKeyDown(KeyCode.V)){
            AcademySB.TwoPlayers = true;
            FindObjectOfType<AcademySB>().resetParameters["twoPlayers"] = 1;
            AcademyReset();
        }
    }

    private void AcademyReset()
    {
        Debug.Log("Reset Academy " + AcademySB.TwoPlayers);
        if (AcademySB.TwoPlayers)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(mPlayer.controller.Player.Team == Team.BLUE);
        }
    }

    private int RaycastAtAngle(float angle, out float distance)
    {
        Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 500, 1 << Layers.GoalBoundaries | 1 << Layers.Walls, QueryTriggerInteraction.Ignore))
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
            Debug.LogError("Should always hit something ");
            distance = 0;
            return 0;
        }
    }

    public override void CollectObservations()
    {
        Vector2 playerPosition = new Vector2(transform.localPosition.x, transform.localPosition.z) / PositionNormalization;

        //player 0-8
        AddVectorObs(playerPosition);
        mPlayer.AddObservation(AddVectorObs, AddVectorObs, AddVectorObs);

        //opponent 9-17
        Vector2 otherPlayerPosition = new Vector2(otherPlayer.transform.localPosition.x, otherPlayer.transform.localPosition.z) / PositionNormalization;
        AddVectorObs(otherPlayerPosition - playerPosition);
        otherPlayer.AddObservation(AddVectorObs, AddVectorObs, AddVectorObs);

        //ball 18-21
        Vector2 ballPosition = new Vector2(MyComponents.BallState.transform.localPosition.x, MyComponents.BallState.transform.localPosition.z) / PositionNormalization;
        AddVectorObs(ballPosition - playerPosition);
        AddVectorObs(new Vector2(MyComponents.BallState.Rigidbody.velocity.x, MyComponents.BallState.Rigidbody.velocity.x) / PositionNormalization);

        //goals 22-29
        foreach (Goal goal in mGoals)
        {
            Vector2 goalPosition = new Vector2(goal.transform.localPosition.x, goal.transform.localPosition.z) / PositionNormalization;
            AddVectorObs(goalPosition - playerPosition);
            AddVectorObs(new Vector2(Mathf.Cos(goal.transform.localRotation.eulerAngles.y), Mathf.Sin(goal.transform.localRotation.eulerAngles.y)));
        }

        AddVectorObs(AcademySB.episodeCompletion);

        //Raycasts 31-62
        int numberOfRaycasts = 16;
        for (int i = 0; i < numberOfRaycasts; i++)
        {
            float angle = (Mathf.PI * 2 / numberOfRaycasts) * i;
            float distance;
            int type = RaycastAtAngle(angle, out distance);
            AddVectorObs(type);
            AddVectorObs(distance / PositionNormalization);
        }
    }

    private StringBuilder LogStuff()
    {
        StringBuilder builder = new StringBuilder();
        if (AcademySB.mode == AcademySB.Mode.PICK_UP)
            builder.AppendLine("HasBall " + mPlayer.controller.Player.HasBall);
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
                mPlayer.controller.inputManager.SetKey(movementKeys[i]);
        }

        if (AcademySB.mode != AcademySB.Mode.PICK_UP)
        {
            if (vectorAction[4] >= threshold)
                mPlayer.controller.inputManager.SetMouseButtonDown(0);
            if (vectorAction[5] >= threshold)
                mPlayer.controller.inputManager.SetMouseButtonUp(0);
        }
        float distance = vectorAction[8] * PositionNormalization;
        mPlayer.controller.inputManager.SetMouseLocalPosition(new Vector3(transform.localPosition.x, 0, transform.localPosition.z) + new Vector3(vectorAction[6], 0, vectorAction[7]) * distance);

        switch (AcademySB.mode)
        {
            case AcademySB.Mode.PICK_UP:
                RewardBallCatches(1f);
                break;
            case AcademySB.Mode.SHOOT:
                RewardShots();
                RewardBallCatches(0.1f, false);
                break;
            case AcademySB.Mode.SHOOT_GOALS:
                RewardGoals();
                RewardBallCatches(0.1f, false);
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
            Debug.Log(builder.ToString());
        }
    }

    private void RewardBallCatches(float amount, bool reset = true)
    {
        if (mPlayer.controller.Player.HasBall && !ballCaughtOnce)
        {
            AddReward(amount);
            if (reset)
            {
                Done();
            }
            else
            {
                ballCaughtOnce = true;
            }
        }
        else
        {
            AddReward(-0.001f);
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
            Done();
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
            SetReward(goalScoredAgainstTeam != mPlayer.controller.Player.Team ? 1 : 0);
            Done();
        }
    }

    private bool resetPlayer = false;

    public override void AgentReset()
    {
        ballCaughtOnce = false;
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
            mPlayer.rigidbody.velocity = Vector3.zero;
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

    void OnDestroy()
    {
        MyComponents.GameInit.AllObjectsCreated -= Init;
        mPlayer.controller.abilitiesManager.PlayerHasShot -= PlayerHasShot;
        foreach (Goal goal in mGoals)
        {
            goal.GoalScored += GoalScored;
        }
    }
}

class PlayerMLData
{
    public delegate void AddVectorObsVec(Vector2 v);
    public delegate void AddVectorObsBool(bool b);
    public delegate void AddVectorObsFloat(float f);
    public Rigidbody rigidbody;
    public PlayerController controller;
    public Transform transform;
    public PowerBar powerBar;

    public PlayerMLData(PlayerController controller)
    {
        this.controller = controller;
        this.rigidbody = controller.mRigidbody;
        this.transform = controller.transform;
        this.powerBar = controller.abilitiesFactory.GetComponentInChildren<PowerBar>();
    }

    public void AddObservation(AddVectorObsVec addVector, AddVectorObsBool addBool, AddVectorObsFloat addFloat)
    {
        addVector(new Vector2(Mathf.Cos(transform.localRotation.eulerAngles.y), Mathf.Sin(transform.localRotation.eulerAngles.y)));
        addVector(new Vector2(rigidbody.velocity.x, rigidbody.velocity.z) / AgentSB.PositionNormalization);
        addBool(controller.Player.HasBall);
        addBool(powerBar.IsFilling());
        addFloat(powerBar.powerValue);
    }
}