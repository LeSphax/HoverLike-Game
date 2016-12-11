using Byn.Net;
using UnityEngine;
using UnityEngine.UI;
using PlayerManagement;
using PlayerBallControl;
using AbilitiesManagement;

[RequireComponent(typeof(PlayerMovementManager))]
[RequireComponent(typeof(PlayerBallController))]
[RequireComponent(typeof(AbilitiesManager))]
public class PlayerController : PlayerView
{
    public GameObject targetPrefab;

    private GameObject Mesh;

    public Text playerName;

    private GameObject _target;
    private GameObject target
    {
        get
        {
            return _target;
        }
        set
        {
            if (value == null)
            {
                movementManager.targetPosition = null;
            }
            else
            {
                movementManager.targetPosition = value.transform.position;
            }
            _target = value;
        }
    }
    [HideInInspector]
    public PlayerMovementManager movementManager;
    [HideInInspector]
    public PlayerBallController ballController;
    [HideInInspector]
    public AbilitiesManager abilitiesManager;

    void Awake()
    {
        movementManager = GetComponent<PlayerMovementManager>();
        abilitiesManager = GetComponent<AbilitiesManager>();
        ballController = GetComponent<PlayerBallController>();
    }

    void LateUpdate()
    {
        if (Player != null && Player.IsMyPlayer)
        {
            GameObject.FindGameObjectWithTag("GameController").transform.position = transform.position;
        }
    }

    internal void DestroyTarget()
    {
        Destroy(target);
        target = null;
    }

    public void CreateTarget()
    {
        Vector3 position = Functions.GetMouseWorldPosition();
        CreateTarget(position);
    }

    public void CreateTarget(Vector3 position)
    {
        DestroyTarget();
        target = (GameObject)Instantiate(targetPrefab, position, Quaternion.identity);
        target.GetComponent<TargetCollisionDetector>().controller = this;
    }

    public void TargetHit()
    {
        DestroyTarget();
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
        Destroy(target);
        target = null;
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
        playerName.text = Player.Nickname;

        CreateMesh();

        ConfigureColliders();

        abilitiesManager.ResetAllEffects();
        MyComponents.AbilitiesFactory.RecreateAbilities();
    }

    private void ConfigureColliders()
    {
        if (MyComponents.NetworkManagement.isServer)
        {
            GetComponent<CapsuleCollider>().radius = Player.MyAvatarSettings.catchColliderRadius;
            GetComponent<CapsuleCollider>().center = Vector3.forward * Player.MyAvatarSettings.catchColliderZPos;
            GetComponent<CapsuleCollider>().height = Player.MyAvatarSettings.catchColliderHeight;
        }
        int layer = -1;
        if (Player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.GOALIE || Players.GetPlayersInTeam(Player.Team).Count == 1)
            layer = LayersGetter.players[(int)Player.Team];
        else if (Players.GetPlayersInTeam(Player.Team).Count == 1)
            layer = LayersGetter.attackers[(int)Player.Team];
        else
            layer = LayersGetter.ATTACKER;
        Functions.SetLayer(transform, layer);
    }

    private void CreateMesh()
    {
        GameObject meshPrefab = Resources.Load<GameObject>(Player.MyAvatarSettings.MESH_NAME);
        Mesh = Instantiate(meshPrefab);
        Mesh.transform.SetParent(transform, false);
        SetMaterials(Mesh);
    }

    private void SetMaterials(GameObject mesh)
    {
        if (Player.IsMyPlayer)
        {
            mesh.transform.GetChild(0).GetComponent<Renderer>().material = ResourcesGetter.OutLineMaterial();
        }
        Color teamColor = Colors.Teams[(int)Player.Team];
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) { if (renderer.tag == Tags.TeamColored) renderer.material.color = teamColor; }
        playerName.color = teamColor;
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
        DestroyTarget();
    }
}
