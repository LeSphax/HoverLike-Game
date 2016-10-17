using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerMovementView))]
public class PlayerController : SlideBall.MonoBehaviour
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

    public Vector3 spawningPoint;
    public int playerNumber;
    public int teamNumber;


    void Start()
    {
        movementManager = GetComponent<PlayerMovementView>();
        target = null;
    }

    void Update()
    {
        if (View.isMine)
        {
            if (Input.GetMouseButton(1))
            {
                CreateTarget();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                ParticleSystem.EmissionModule em = transform.GetChild(1).GetComponent<ParticleSystem>().emission;
                em.enabled = true;
                //transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                transform.GetChild(1).GetComponent<ParticleSystem>().Emit(1);
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

    public void Init(int teamNumber, string name)
    {
        spawningPoint = GameObject.FindGameObjectWithTag(Tags.Spawns).transform.GetChild(teamNumber).position;
        View.RPC("InitPlayer", RPCTargets.AllBuffered, teamNumber, name, spawningPoint);
    }

    [MyRPC]
    private void InitPlayer(int teamNumber, string name, Vector3 spawningPoint)
    {
        if (View.isMine)
        {
            tag = Tags.MyPlayer;
        }
        this.teamNumber = teamNumber;
        gameObject.name = name;
        this.spawningPoint = spawningPoint;
        transform.position = spawningPoint;
        transform.LookAt(Vector3.zero);

        if (teamNumber == 0)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) { renderer.material = ResourcesGetter.BlueMaterial(); }
        }
        else
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) { renderer.material = ResourcesGetter.RedMaterial(); }
        }
        gameObject.layer = LayersGetter.players[teamNumber];
        foreach (Transform go in transform) { go.gameObject.layer = LayersGetter.players[teamNumber]; };
    }
}
