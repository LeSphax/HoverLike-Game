using System;
using UnityEngine;

public class BlockEffect : AbilityEffect
{

    public float radius = 100f;
    public float power = 100.0f;

    public GameObject explosionPrefab;
    private GameObject sphereExplosion;

    public override void ApplyOnTarget(GameObject target, Vector3 position)
    {
        AddForces(position);
        VisualEffects(target.transform);
    }

    private void VisualEffects(Transform target)
    {
        sphereExplosion = (GameObject)Instantiate(explosionPrefab, Vector3.zero, Quaternion.identity);
        sphereExplosion.transform.SetParent(target, false);
        ScaleAnimation animation = ScaleAnimation.CreateScaleAnimation(sphereExplosion, 0.1f, 5f, 0.3f);
        animation.FinishedAnimating += DestroySphere;
        animation.StartAnimating();
    }

    private void DestroySphere(MonoBehaviour sender)
    {
        Destroy(sphereExplosion);
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
