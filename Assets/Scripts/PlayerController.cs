using UnityEngine;

public class PlayerController : Photon.MonoBehaviour
{
    public GameObject targetPrefab;
    private GameObject target;
    PhotonTransformView rigidbodyView;

    public Transform spawningPoint;
    public int playerNumber;
    public int teamNumber;

    Vector3 velocity;

    void Start()
    {
        velocity = Vector3.zero;
        target = null;
    }

    void FixedUpdate()
    {
        if (photonView.isMine)
        {
            if (Input.GetMouseButton(1))
            {
                CreateTarget();
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                ParticleSystem.EmissionModule em = transform.GetChild(1).GetComponent<ParticleSystem>().emission;
                em.enabled = true;
                //transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                transform.GetChild(1).GetComponent<ParticleSystem>().Emit(1);
            }
            if (target != null)
            {

                transform.LookAt(target.transform.position + Vector3.up * transform.position.y);
                Vector3 deltaVelocity = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
                deltaVelocity.Normalize();
                velocity += deltaVelocity;
                GetComponent<Rigidbody>().AddForce(deltaVelocity, ForceMode.VelocityChange);
            }
        }
    }

    private void CreateTarget()
    {
        Vector3 position = Functions.GetMouseWorldPosition();
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
        spawningPoint = GameObject.FindGameObjectWithTag(Tags.Spawns).transform.GetChild(playerNumber - 1);
        photonView.RPC("InitPlayer",PhotonTargets.AllBufferedViaServer,playerNumber,name,spawningPoint);
    }

    [PunRPC]
    private void InitPlayer(int playerNumber, string name, Transform spawningPoint)
    {
        this.playerNumber = playerNumber;
        gameObject.name = name;
        this.spawningPoint = spawningPoint;
    }
}
