using Byn.Net;
using UnityEngine;
using PlayerManagement;
using PlayerBallControl;
using AbilitiesManagement;
using UnityEngine.Assertions;

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
        Player.controller = this;
        Player.gameobjectAvatar = gameObject;
        Player.ballController = GetComponent<PlayerBallController>();
        GetComponent<PlayerBallController>().Init(playerConnectionId);
    }

    [MyRPC]
    public void ResetPlayer()
    {
        movementManager.Reset(playerConnectionId);
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

        gameObject.name = Player.Nickname;
        billboard.Text = Player.Nickname;
        billboard.SetHeight(16);

        CreateMesh();

        ConfigureColliders();

        abilitiesManager.ResetAllEffects();
        MyComponents.AbilitiesFactory.RecreateAbilities();
    }

    private void ConfigureColliders()
    {
        if (MyComponents.NetworkManagement.isServer)
        {
            //GetComponent<CapsuleCollider>().radius = Player.MyAvatarSettings.catchColliderRadius;
            //GetComponent<CapsuleCollider>().center = Vector3.forward * Player.MyAvatarSettings.catchColliderZPos;
            //GetComponent<CapsuleCollider>().height = Player.MyAvatarSettings.catchColliderHeight;
        }
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
        transform.LookAt(Vector3.zero);
        if (MyComponents.NetworkManagement.isServer)
            StopMoving();
    }

    public void StopMoving()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        targetManager.CancelTarget();
    }
}
