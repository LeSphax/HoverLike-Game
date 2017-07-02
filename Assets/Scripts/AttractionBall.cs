using System.Collections.Generic;
using UnityEngine;

public class AttractionBall : MonoBehaviour
{

    public float power = 2f;

    List<GameObject> playersAttracting = new List<GameObject>();

    public static bool Activated = true;

    void OnTriggerEnter(Collider collider)
    {
        if (LayersGetter.IsAttacker(collider.gameObject.layer))
        {
            playersAttracting.Add(collider.gameObject);
        }
    }

    public void Reset()
    {
        playersAttracting.Clear();
    }

    void OnTriggerExit(Collider collider)
    {
        if (LayersGetter.IsAttacker(collider.gameObject.layer))
            playersAttracting.Remove(collider.gameObject);
    }

    public void RemovePlayer(GameObject player)
    {
        playersAttracting.Remove(player);
    }

    void Update()
    {
        if (!MyComponents.BallState.IsAttached() && !MyComponents.BallState.UnCatchable && Activated)
            foreach (GameObject player in playersAttracting)
            {
                Vector3 target = player.transform.position;
                Vector3 velocity = new Vector3(target.x - transform.position.x, 0, target.z - transform.position.z);
                velocity.Normalize();
                transform.parent.GetComponent<Rigidbody>().velocity += velocity * power;
            }
    }
}
