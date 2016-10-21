using Byn.Net;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovementView))]
public class PlayerController : PlayerView
{
    public GameObject targetPrefab;
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

        if (Player.Team == Team.FIRST)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) { renderer.material = ResourcesGetter.BlueMaterial(); }
        }
        else
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) { renderer.material = ResourcesGetter.RedMaterial(); }
        }
        gameObject.layer = LayersGetter.players[(int)Player.Team];
        foreach (Transform go in transform) { go.gameObject.layer = LayersGetter.players[(int)Player.Team]; };
    }

    [MyRPC]
    public void PutAtStartPosition()
    {
        if (View.isMine)
        {
            transform.position = MyGameObjects.Spawns.GetSpawn(Player.Team, Player.SpawningPoint);
            transform.LookAt(Vector3.zero);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            DestroyTarget();
        }
        else if (MyGameObjects.NetworkManagement.isServer)
        {
            View.RPC("PutAtStartPosition", connectionId);
        }
        else
        {
            Debug.LogError("This function shouldn't be called on a client that doesn't own the view");
        }
    }
}
