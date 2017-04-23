using UnityEngine;
using System.Collections;

public class Shockwaves : MonoBehaviour
{

    public Shockwave[] shockwaves;
    AudioSource m_audio;
    private int finished = 0;
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < shockwaves.Length; i++)
        {
            shockwaves[i].FinishedAnimating += (sender) => Finished(i);
        }
    }

    private void Finished(int i)
    {
        finished++;
        if (finished == shockwaves.Length)
        {
            Destroy(gameObject);
        }
    }

    public void InitView(object[] parameters)
    {
        bool Landing = (bool)parameters[0];
        m_audio = GetComponent<AudioSource>();
        if (Landing)
        {
            m_audio.clip = ResourcesGetter.LandingSound;
        }
        else
        {
            m_audio.clip = ResourcesGetter.JumpingSound;
        }
        m_audio.Play();
    }
}
