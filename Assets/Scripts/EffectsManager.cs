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
        PlayClipAtPoint(ResourcesGetter.PassSound(), controller.transform.position, 0.5f);
    }

    [MyRPC]
    public void ShowSmoke()
    {
        BlueSmoke.Play();
        PlayClipAtPoint(ResourcesGetter.BoostSound(), controller.transform.position, 0.2f);
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

    void PlayClipAtPoint(AudioClip clip, Vector3 point, float volume)
    {
        GameObject source = Instantiate(ResourcesGetter.TempAudioSource);
        source.transform.position = point;
        source.GetComponent<AudioSource>().clip = clip;
        source.GetComponent<AudioSource>().volume = volume;
        source.GetComponent<AudioSource>().Play();
    }

}
