using System.Collections.Generic;
using UnityEngine;

public class AttractionBall : MonoBehaviour
{

    public float power = 2f;
    public static bool activated = true;

    List<GameObject> playersAttracting = new List<GameObject>();

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log(Tags.IsPlayer(collider.gameObject.tag));
        if (Tags.IsPlayer(collider.gameObject.tag))
            playersAttracting.Add(collider.gameObject);
    }

    void OnTriggerExit(Collider collider)
    {
        Debug.Log(Tags.IsPlayer(collider.gameObject.tag));
        if (Tags.IsPlayer(collider.gameObject.tag))
        {
            playersAttracting.Remove(collider.gameObject);
        }
    }

    void Update()
    {
        if (!BallState.IsAttached() && activated)
            foreach (GameObject player in playersAttracting)
            {
                Vector3 target = player.transform.position;
                Vector3 velocity = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
                velocity.Normalize();
                transform.parent.GetComponent<Rigidbody>().velocity += velocity * power;
            }
    }
}
