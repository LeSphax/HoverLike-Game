using UnityEngine;

public class EffectsManager : SlideBall.MonoBehaviour
{

    public ParticleSystem BlueSmoke;
    public ParticleSystem Slow;

    [MyRPC]
    public void ShowSmoke()
    {
        BlueSmoke.Play();
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
