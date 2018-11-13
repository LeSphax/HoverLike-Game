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
    public event EmptyEventHandler Reset;

    private GameObject Mesh;

    public BillBoard billboard;

    [HideInInspector]
    public InputManager inputManager;
    [HideInInspector]
    public TargetManager targetManager;
    [HideInInspector]
    public PlayerMovementManager movementManager;
    [HideInInspector]
    public PlayerBallController ballController;
    [HideInInspector]
    public AbilitiesManager abilitiesManager;
    public AbilitiesFactory abilitiesFactory;
    public Rigidbody mRigidbody;
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
        inputManager = GetComponent<InputManager>();
        targetManager = GetComponent<TargetManager>();
        movementManager = GetComponent<PlayerMovementManager>();
        abilitiesManager = GetComponent<AbilitiesManager>();
        ballController = GetComponent<PlayerBallController>();
        mRigidbody = GetComponent<Rigidbody>();
    }

    public void InitView(object[] parameters)
    {
        PlayerConnectionId = (ConnectionId)parameters[0];
        if (Player == null)
        {
            Destroy(gameObject);
            return;
        }
        Player.controller = this;
        Player.ballController = GetComponent<PlayerBallController>();
        GetComponent<PlayerBallController>().Init(PlayerConnectionId);
        MyComponents.Players.NotifyNewPlayerInstantiated(PlayerConnectionId);

        Player.eventNotifier.ListenToEvents(ResetPlayer, PlayerFlags.TEAM, PlayerFlags.AVATAR_SETTINGS);
        Player.eventNotifier.ListenToEvents(SetNickname, PlayerFlags.NICKNAME);
        Player.eventNotifier.ListenToEvents(Destroy, PlayerFlags.DESTROYED);
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
        movementManager.Reset(PlayerConnectionId);
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

        CreateAbilities();

        abilitiesManager.ResetAllEffects();

        if (Reset != null)
            Reset.Invoke();
    }

    private void CreateAbilities()
    {
        if (abilitiesFactory == null)
        {
            GameObject abilitiesPrefab = Resources.Load<GameObject>(Paths.ABILITIES_GAMEOBJECT);
            GameObject abilities = GameObject.Instantiate(abilitiesPrefab, MyComponents.UI().transform);
            abilities.name = Player.Nickname + "Abilities";
            abilitiesFactory = abilities.GetComponent<AbilitiesFactory>();
            abilitiesFactory.PlayerId = Player.id;
        }
        abilitiesFactory.RecreateAbilities();
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
        mRigidbody.velocity = Vector3.zero;
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
