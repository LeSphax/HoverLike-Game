using Byn.Net;
using PlayerManagement;
using UnityEngine;

public class EffectsManager : SlideBall.MonoBehaviour
{

    public ParticleSystem BlueSmoke;
    public ParticleSystem Slow;

    [MyRPC]
    public void ThrowBall(ConnectionId id)
    {
        Players.players[id].controller.animator.SetTrigger("Throw");
    }

    [MyRPC]
    public void ShowSmoke()
    {
        BlueSmoke.Play();
    }

    [MyRPC]
    public void ShowStealing(float duration)
    {
        GetComponentInChildren<StealRenderer>().StartAnimating(duration);
    }

    public void ShockwaveOnPlayer(bool landing)
    {
        MyComponents.NetworkViewsManagement.Instantiate("Effects/Shockwave", transform.position, Quaternion.identity, landing);
    }

    public void ActivateSlow(bool activate)
    {
        if (activate && !Slow.isPlaying)
        {
            Slow.Play();
        }
        else if (!activate && Slow.isPlaying)
        {
            Slow.Stop();
        }
    }

}
