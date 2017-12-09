using Byn.Net;
using PlayerManagement;
using UnityEngine;

public class BlockExplosion : MonoBehaviour
{
    public const float BLOCK_DIAMETER = 35;

    private ConnectionId targetId;
    private Transform target;

    // Use this for initialization
    void Start()
    {
        VisualEffects(target.transform);
    }

    public void InitView(object[] parameters)
    {
        targetId = (ConnectionId)parameters[0];
        target = Players.players[targetId].controller.transform;
    }

    private void VisualEffects(Transform target)
    {
        transform.SetParent(target, false);
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        Invoke("DestoySphere", 1); 
    }

    private void DestroySphere()
    {
        Destroy(gameObject);
    }

}
