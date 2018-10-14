using Byn.Net;
using UnityEngine;
using PlayerManagement;
using PlayerBallControl;
using AbilitiesManagement;
using UnityEngine.Assertions;
using Ball;

[RequireComponent(typeof(PlayerMovementManager))]
[RequireComponent(typeof(PlayerBallController))]
[RequireComponent(typeof(AbilitiesManager))]
public class PlayerController : PlayerView
{
    private GameObject Mesh;

    public BillBoard billboard;

    [HideInInspector]
    public TargetManager targetManager;
    [HideInInspector]
    public PlayerMovementManager movementManager;
    [HideInInspector]
    public PlayerBallController ballController;
    [HideInInspector]
    public AbilitiesManager abilitiesManager;
    private PlayerMesh playerMesh;
    public PlayerMesh PlayerMesh
    {
        get
        {
            return playerMesh;
        }
        set
        {
            playerMesh = value;
            if (playerMesh == null)
            {
                animator = null;
            }
            else
            {
                animator = playerMesh.GetComponentInChildren<Animator>();
            }
        }
    }
    [HideInInspector]
    public Animator animator;

    void Awake()
    {
        movementManager = GetComponent<PlayerMovementManager>();
        abilitiesManager = GetComponent<AbilitiesManager>();
        ballController = GetComponent<PlayerBallController>();
        targetManager = GetComponent<TargetManager>();
    }

    public void InitView(object[] parameters)
    {
        playerConnectionId = (ConnectionId)parameters[0];
        if (Player == null)
        {
            Destroy(gameObject);
            return;
        }
        Player.controller = this;
        Player.ballController = GetComponent<PlayerBallController>();
        GetComponent<PlayerBallController>().Init(playerConnectionId);
        MyComponents.Players.NotifyNewPlayerInstantiated(playerConnectionId);

        Player.eventNotifier.ListenToEvents(ResetPlayer, PlayerFlags.TEAM, PlayerFlags.AVATAR_SETTINGS);
        Player.eventNotifier.ListenToEvents(SetNickname, PlayerFlags.NICKNAME);
        Player.eventNotifier.ListenToEvents(Destroy, PlayerFlags.DESTROYED);

        ResetPlayer();
    }

    [MyRPC]
    public void ResetPlayer(Player player = null)
    {
        if (Player.Team == Team.NONE || Player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.NONE)
        {
            Debug.LogWarning("ResetPlayer : Player has no team or avatar " + Player.Team + "   " + Player.AvatarSettingsType);
            return;
        }
        if (Player.HasBall)
        {
            if (NetworkingState.IsServer)
            {
                MyComponents.BallState.PutAtStartPosition();
            }
            else
            {
                MyComponents.BallState.AttachBall(BallState.NO_PLAYER_ID);
            }
        }

        RemoveFromAttraction();
        movementManager.Reset(playerConnectionId);
        ballController.Reset();
        targetManager.CancelTarget();
        if (Mesh != null)
        {
            Destroy(Mesh);
        }
        if (Player.IsMyPlayer)
        {
            tag = Tags.MyPlayer;
        }
        PutAtStartPosition();

        SetNickname();
        billboard.SetHeight(16);

        CreateMesh();

        ConfigureColliders();

        abilitiesManager.ResetAllEffects();

        if (Player.IsMyPlayer)
            MyComponents.AbilitiesFactory.RecreateAbilities();
    }

    private void RemoveFromAttraction()
    {
        if (NetworkingState.IsServer && PlayerMesh != null)
            MyComponents.BallState.attraction.RemovePlayer(PlayerMesh.actualCollider.gameObject);
    }

    private void SetNickname(Player player = null)
    {
        gameObject.name = Player.Nickname;
        billboard.Text = Player.Nickname;
    }

    private void ConfigureColliders()
    {
        int layer = -1;
        if (Player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.GOALIE)
            layer = LayersGetter.players[(int)Player.Team];
        else
            layer = LayersGetter.attackers[(int)Player.Team];
        Functions.SetLayer(transform, layer);
    }

    private void CreateMesh()
    {
        GameObject meshPrefab = Resources.Load<GameObject>(Player.MyAvatarSettings.MESH_NAME);
        Mesh = Instantiate(meshPrefab);
        PlayerMesh = Mesh.GetComponent<PlayerMesh>();
        Mesh.transform.SetParent(transform, false);
        SetMaterials(Mesh);
    }

    private void SetMaterials(GameObject mesh)
    {
        Assert.IsNotNull(PlayerMesh);
        Assert.IsNotNull(Player);
        PlayerMesh.SetOwner(Player.IsMyPlayer);
        Color teamColor = Colors.Teams[(int)Player.Team];
        PlayerMesh.SetTeam(Player.Team);
        billboard.Color = teamColor;
    }

    public void PutAtStartPosition()
    {
        transform.position = Player.SpawningPoint;
        transform.LookAt(transform.parent.localPosition);
        transform.localRotation = Quaternion.Euler(0, transform.localRotation.y, 0);
        if (NetworkingState.IsServer)
            StopMoving();
    }

    public void StopMoving()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        targetManager.CancelTarget();
    }

    [MyRPC]
    public void SetStealOnCooldown()
    {
        GameObject go;
        if (MyComponents.AbilitiesFactory.abilityGOs.TryGetValue("Steal", out go))
            go.GetComponentInChildren<Ability>().SetOnCooldown();
    }

    private void Destroy(Player player = null)
    {
        RemoveFromAttraction();
        Destroy(gameObject);
        Player.UpdatePlayersDataOnDestroy(player.id, MyComponents);
    }

    private void OnDestroy()
    {
        if (Player != null)
        {
            Player.eventNotifier.StopListeningToEvents(ResetPlayer, PlayerFlags.TEAM, PlayerFlags.AVATAR_SETTINGS);
            Player.eventNotifier.StopListeningToEvents(SetNickname, PlayerFlags.NICKNAME);
            Player.eventNotifier.StopListeningToEvents(Destroy, PlayerFlags.DESTROYED);
        }
    }
}
