using UnityEngine;
using System.Collections;
using PlayerManagement;
using Byn.Net;
using AbilitiesManagement;
using CustomAnimations;

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
    }

    private void VisualEffects(Transform target)
    {
        transform.SetParent(target, false);
        ScaleAnimation animation = ScaleAnimation.CreateScaleAnimation(gameObject, 10f, BLOCK_DIAMETER, 0.4f);
        animation.FinishedAnimating += DestroySphere;
        animation.StartAnimating();

        GetComponentsFadeAnimation fadeAnimation = GetComponentsFadeAnimation.CreateGetComponentsFadeAnimation(gameObject, 0.2f);
        fadeAnimation.FinishedAnimating += Disappear;
        fadeAnimation.StartAnimating();
    }

    private void Disappear(MyAnimation sender)
    {
        sender.StartReverseAnimating();
    }

    private void DestroySphere(MyAnimation sender)
    {
        Destroy(gameObject);
    }

}
