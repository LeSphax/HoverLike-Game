using UnityEngine;
using System.Collections;
using PlayerManagement;
using Byn.Net;
using AbilitiesManagement;

public class BlockExplosion : MonoBehaviour
{
    public const float BLOCK_DIAMETER = 35;
    public float power = 100.0f;

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
        AbilitiesManager.visualEffects.Add(gameObject);
    }

    private void VisualEffects(Transform target)
    {
        transform.SetParent(target, false);
        ScaleAnimation animation = ScaleAnimation.CreateScaleAnimation(gameObject, 20f, BLOCK_DIAMETER, 0.2f);
        animation.FinishedAnimating += DestroySphere;
        animation.StartAnimating();
    }

    private void DestroySphere(MonoBehaviour sender)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        AbilitiesManager.visualEffects.Remove(gameObject);
    }

}
