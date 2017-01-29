using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class EffectsManager : SlideBall.MonoBehaviour
{

    public ParticleSystem BlueSmoke;
    public ParticleSystem Braking;

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
        if (activate && !Braking.isPlaying)
        {
            Braking.Play();
        }
        else if (!activate && Braking.isPlaying)
        {
            Braking.Stop();
        }
    }

}
