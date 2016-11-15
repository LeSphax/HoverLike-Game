﻿using Byn.Net;
using UnityEngine;
using UnityEngine.UI;
using PlayerManagement;
using PlayerBallControl;
using AbilitiesManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovementView))]
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
    PlayerMovementView movementManager;
    public AbilitiesManager abilitiesManager;

    void Awake()
    {
        movementManager = GetComponent<PlayerMovementView>();
        abilitiesManager = GetComponent<AbilitiesManager>();
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

    [MyRPC]
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

    public void Init(ConnectionId id)
    {
        View.RPC("InitPlayer", RPCTargets.AllBuffered, id);
    }

    [MyRPC]
    private void InitPlayer(ConnectionId id)
    {
        playerConnectionId = id;
        Player.controller = this;
        Player.gameobjectAvatar = gameObject;
        Player.ballController = GetComponent<PlayerBallController>();
        GetComponent<PlayerBallController>().Init(id);
        ResetPlayer();
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

        MyComponents.AbilitiesFactory.RecreateAbilities();
    }

    private void ConfigureColliders()
    {
        GetComponent<CapsuleCollider>().radius = Player.MyAvatarSettings.catchColliderRadius;
        GetComponent<CapsuleCollider>().center = Vector3.forward * Player.MyAvatarSettings.catchColliderZPos;
        GetComponent<CapsuleCollider>().height = Player.MyAvatarSettings.catchColliderHeight;
        int layer = -1;
        //if (Player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.GOALIE)
        layer = LayersGetter.players[(int)Player.Team];
        //else
        //    layer = LayersGetter.players[2];
        Functions.SetLayer(Mesh.transform, layer);
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
        transform.position = MyComponents.Spawns.GetSpawn(Player.Team, Player.SpawnNumber);
        transform.LookAt(Vector3.zero);
        StopMoving();
    }

    public void StopMoving()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        DestroyTarget();
    }
}
