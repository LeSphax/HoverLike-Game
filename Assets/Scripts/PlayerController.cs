using System;
using UnityEngine;

public class PlayerController : Photon.MonoBehaviour
{
    public GameObject targetPrefab;
    private GameObject target;
    PhotonTransformView rigidbodyView;

    public Vector3 spawningPoint;
    public int playerNumber;
    public int teamNumber;

    private Rigidbody m_rigidbody;

    private const float MAX_VELOCITY = 100f;

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
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

    private void Shoot()
    {

    }

    private void LoadShoot()
    {
    }

    void FixedUpdate()
    {
        if (photonView.isMine)
        {
            if (target != null)
            {
                m_rigidbody.drag = 2;
                transform.LookAt(target.transform.position + Vector3.up * transform.position.y);
                Vector3 force = new Vector3(target.transform.position.x - transform.position.x, 0, target.transform.position.z - transform.position.z);
                force.Normalize();
                m_rigidbody.AddForce(force * 2, ForceMode.VelocityChange);
            }
            else
            {
                m_rigidbody.drag = 1;
            }
            GetComponent<Rigidbody>().velocity *= Mathf.Min(1.0f, MAX_VELOCITY / m_rigidbody.velocity.magnitude);
        }
    }

    public void CreateTarget()
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
