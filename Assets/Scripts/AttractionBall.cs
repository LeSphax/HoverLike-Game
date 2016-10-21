using System.Collections.Generic;
using UnityEngine;

public class AttractionBall : MonoBehaviour
{

    public float power = 2f;

    static List<GameObject> deactivatedPlayers = new List<GameObject>();
    List<GameObject> playersAttracting = new List<GameObject>();

    void OnTriggerEnter(Collider collider)
    {
        if (Tags.IsPlayer(collider.gameObject.tag))
            playersAttracting.Add(collider.gameObject);
    }

    void OnTriggerExit(Collider collider)
    {
        if (Tags.IsPlayer(collider.gameObject.tag))
        {
            playersAttracting.Remove(collider.gameObject);
        }
    }

    public static void ActivatePlayer(GameObject player)
    {
        deactivatedPlayers.Remove(player);
    }

    public static void DeactivatePlayer(GameObject player)
    {
        deactivatedPlayers.Add(player);
    }

    void Update()
    {
        if (!MyGameObjects.BallState.IsAttached())
            foreach (GameObject player in playersAttracting)
            {
                if (!deactivatedPlayers.Contains(player))
                {
                    Vector3 target = player.transform.position;
                    Vector3 velocity = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
                    velocity.Normalize();
                    transform.parent.GetComponent<Rigidbody>().velocity += velocity * power;
                }
            }
    }
}
