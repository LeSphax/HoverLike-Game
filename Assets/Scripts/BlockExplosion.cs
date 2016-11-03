using UnityEngine;
using System.Collections;
using PlayerManagement;
using Byn.Net;

public class BlockExplosion : MonoBehaviour
{
    public float radius = 35f;
    public float power = 100.0f;

    private ConnectionId targetId;
    private Transform target;

    // Use this for initialization
    void Start()
    {
        if (MyComponents.NetworkManagement.isServer)
            AddForces(target.position);
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
        ScaleAnimation animation = ScaleAnimation.CreateScaleAnimation(gameObject, 20f, 35f, 0.2f);
        animation.FinishedAnimating += DestroySphere;
        animation.StartAnimating();
    }

    private void DestroySphere(MonoBehaviour sender)
    {
        Destroy(gameObject);
    }

    private void AddForces(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius, LayersGetter.BallMask());
        foreach (Collider hit in colliders)
        {
            if (hit.tag == Tags.Ball)
            {
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                Vector3 force = hit.transform.position - position;
                force.Normalize();
                if (rb != null)
                {
                    rb.AddForce(force * power, ForceMode.VelocityChange);
                }

            }
        }
    }
}
