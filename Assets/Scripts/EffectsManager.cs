using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class EffectsManager : SlideBall.MonoBehaviour
{

    public ParticleSystem BlueSmoke;
    public TrailRenderer Braking;

    private PlayerController controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        Braking.enabled = false;
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
        if (activate != Braking.enabled)
            Braking.enabled = activate;
    }

}
