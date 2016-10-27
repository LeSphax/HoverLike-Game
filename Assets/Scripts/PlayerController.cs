using Byn.Net;
using UnityEngine;
using UnityEngine.UI;
using PlayerManagement;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovementView))]
public class PlayerController : PlayerView
{
    public GameObject targetPrefab;

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

    void Awake()
    {
        movementManager = GetComponent<PlayerMovementView>();
        target = null;
    }

    void Update()
    {
        if (View.isMine)
        {
            if (Input.GetMouseButton(1) && MyGameObjects.MatchManager.CanPlay)
            {
                CreateTarget();
            }
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
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == Tags.Target)
        {
            DestroyTarget();
        }
    }

    public void Init(ConnectionId id, int teamNumber, string name)
    {
        View.RPC("InitPlayer", RPCTargets.AllBuffered, id);
    }

    [MyRPC]
    private void InitPlayer(ConnectionId id)
    {
        if (View.isMine)
        {
            tag = Tags.MyPlayer;
            PutAtStartPosition();
        }
        connectionId = id;
        gameObject.name = Player.Nickname;
        playerName.text = Player.Nickname;

        SetMaterials();
        gameObject.layer = LayersGetter.players[(int)Player.Team];
        foreach (Transform go in transform) { go.gameObject.layer = LayersGetter.players[(int)Player.Team]; };
    }

    private void SetMaterials()
    {
        if (View.isMine)
        {
            GetComponent<Renderer>().material = ResourcesGetter.OutLineMaterial();
        }
        Color teamColor = Colors.Teams[(int)Player.Team];
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) { renderer.material.color = teamColor; }
        playerName.color = teamColor;
    }

    [MyRPC]
    public void PutAtStartPosition()
    {

        transform.position = MyGameObjects.Spawns.GetSpawn(Player.Team, Player.SpawningPoint);
        transform.LookAt(Vector3.zero);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        DestroyTarget();
        //else
        //{
        //    Debug.LogError("This function shouldn't be called on a client that doesn't own the view");
        //}
    }

    public void CallPutAtStartPosition()
    {
        View.RPC("PutAtStartPosition", RPCTargets.All);
    }
}
