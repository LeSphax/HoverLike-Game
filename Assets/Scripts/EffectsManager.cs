using PlayerBallControl;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class EffectsManager : SlideBall.MonoBehaviour
{

    public ParticleSystem BlueSmoke;
    public TrailRenderer Braking;
    public ProtectionFX ProtectionCapsule;

    private PlayerController controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        Braking.enabled = false;
    }

    [MyRPC]
    public void ThrowBall()
    {
        if (controller.Player.IsMyPlayer)
            ActualAbilitiesLatency.Received(typeof(ShootEffect), typeof(PassEffect));
        controller.animator.SetTrigger("Throw");
        PlayClipAtPoint(ResourcesGetter.PassSound, controller.transform.position, 0.5f);
        ShowArmAnimation(false);
    }

    [MyRPC]
    public void ShowArmAnimation(bool isArming)
    {
        controller.animator.SetBool("Arming", isArming);
    }

    [MyRPC]
    public void ShowSmoke()
    {
        if (controller.Player.IsMyPlayer)
            ActualAbilitiesLatency.Received(typeof(DashEffect));
        var col = BlueSmoke.colorOverLifetime;
        col.enabled = true;

        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.red, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });

        col.color = grad;

        BlueSmoke.colorOverLifetime =  new ParticleSystem.ColorOverLifetimeModule()
        BlueSmoke.Play();
        PlayClipAtPoint(ResourcesGetter.BoostSound, controller.transform.position, 0.2f);
    }

    [MyRPC]
    public void ShowStealing()
    {
        if (controller.Player.IsMyPlayer)
            ActualAbilitiesLatency.Received(typeof(StealEffect));
        controller.animator.SetTrigger("Steal");
    }

    //TODO: State effects should be linked to an event notifier on Player.State
    [MyRPC]
    public void ShowProtection()
    {
        if (controller.Player.IsMyPlayer)
            ActualAbilitiesLatency.Received(typeof(StealEffect));
        ProtectionCapsule.ShowProtection();
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

    void PlayClipAtPoint(AudioClip clip, Vector3 point, float volume)
    {
        GameObject source = Instantiate(ResourcesGetter.TempAudioSourcePrefab);
        source.transform.position = point;
        AudioSource audioSource = source.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
    }

}
