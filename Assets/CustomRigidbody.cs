using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomRigidbody : MonoBehaviour
{

    [HideInInspector]
    public Vector3 velocity;

    private Rigidbody myRigidbody;
    public float drag;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    public void CustomUpdate()
    {
        Vector3 nextPosition = transform.position + velocity * Time.deltaTime;
        Vector3 movement = nextPosition - transform.position;
        RaycastHit[] hitInfos;
        bool collision = false;
        if ((hitInfos = myRigidbody.SweepTestAll(movement, movement.magnitude)).Length > 0)
        {
            foreach (RaycastHit hit in hitInfos)
            {
                if (!hit.collider.isTrigger)
                    collision = true;
            }
        }
        if (!collision)
        {
            transform.position = nextPosition;
        }
        velocity = velocity * (1 - Time.deltaTime * drag);
    }

    public void AddForce(Vector3 force)
    {
        velocity += force;
    }
}
