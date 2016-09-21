using UnityEngine;

[RequireComponent(typeof(CustomRigidbody))]
[RequireComponent(typeof(PlayerMovementPhotonView))]
public class PlayerController : Photon.MonoBehaviour
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
    PhotonTransformView rigidbodyView;
    PlayerMovementPhotonView movementManager;

    public Vector3 spawningPoint;
    public int playerNumber;
    public int teamNumber;

    private CustomRigidbody myRigidbody;

    void Start()
    {
        movementManager = GetComponent<PlayerMovementPhotonView>();
        myRigidbody = GetComponent<CustomRigidbody>();
        target = null;
    }

    void Update()
    {
        if (photonView.isMine)
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
    }

    public void CreateTarget()
    {
        Vector3 position = Functions.GetMouseWorldPosition();
        Destroy(target);
        target = (GameObject)Instantiate(targetPrefab, position, Quaternion.identity);
        //photonView.RPC("_CreateTarget", PhotonTargets.AllBufferedViaServer, position);
    }

    [PunRPC]
    public void _CreateTarget(Vector3 position)
    {
        Destroy(target);
        target = (GameObject)Instantiate(targetPrefab, position, Quaternion.identity);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == Tags.Target)
        {
            Destroy(target);
            target = null;
        }
    }

    public void Init(int playerNumber, string name)
    {
        spawningPoint = GameObject.FindGameObjectWithTag(Tags.Spawns).transform.GetChild(playerNumber - 1).position;
        photonView.RPC("InitPlayer", PhotonTargets.AllBufferedViaServer, playerNumber, name, spawningPoint);
    }

    [PunRPC]
    private void InitPlayer(int playerNumber, string name, Vector3 spawningPoint)
    {
        if (photonView.isMine)
        {
            tag = Tags.MyPlayer;
        }
        this.playerNumber = playerNumber;
        gameObject.name = name;
        this.spawningPoint = spawningPoint;
    }
}
