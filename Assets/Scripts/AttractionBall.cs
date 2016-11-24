using System.Collections.Generic;
using UnityEngine;

public class AttractionBall : MonoBehaviour
{

    public float power = 2f;

    static List<GameObject> deactivatedPlayers = new List<GameObject>();
    List<GameObject> playersAttracting = new List<GameObject>();

    void OnTriggerEnter(Collider collider)
    {
        if (IsAttacker(collider))
            playersAttracting.Add(collider.gameObject);
    }

    public void Reset()
    {
        deactivatedPlayers.Clear();
        playersAttracting.Clear();
    }

    private bool IsAttacker(Collider collider)
    {
        if (!Tags.IsPlayer(collider.gameObject.tag))
            return false;
        PlayerController controller = collider.gameObject.GetComponent<PlayerController>();
        return controller.Player != null && controller.Player.AvatarSettingsType == AvatarSettings.AvatarSettingsTypes.ATTACKER;
    }

    void OnTriggerExit(Collider collider)
    {
        if (IsAttacker(collider))
            playersAttracting.Remove(collider.gameObject);
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
        if (!MyComponents.BallState.IsAttached() && !MyComponents.BallState.UnPickable)
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
