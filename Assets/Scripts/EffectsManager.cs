using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class EffectsManager : SlideBall.MonoBehaviour
{

    public ParticleSystem BlueSmoke;
    public ParticleSystem Slow;

    private PlayerController controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    [MyRPC]
    public void ThrowBall()
    {
        controller.animator.SetTrigger("Throw");
    }

    [MyRPC]
    public void ShowSmoke()
    {
        BlueSmoke.Play();
    }

    [MyRPC]
    public void ShowStealing()
    {
        controller.animator.SetTrigger("Steal");
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
