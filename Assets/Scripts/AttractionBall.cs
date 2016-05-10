using System.Collections.Generic;
using UnityEngine;

public class AttractionBall : MonoBehaviour
{

    public float speed = 2f;

    List<GameObject> playersAttracting = new List<GameObject>();

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == Tags.Player)
            playersAttracting.Add(collider.gameObject);
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == Tags.Player)
            playersAttracting.Remove(collider.gameObject);
    }

    void Update()
    {
        if (!BallState.IsAttached())
            foreach (GameObject player in playersAttracting)
            {
                Vector3 target = player.transform.position;
                Vector3 velocity = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
                velocity.Normalize();
                transform.parent.GetComponent<Rigidbody>().velocity += velocity * speed;
            }
    }
}
