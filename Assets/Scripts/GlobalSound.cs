using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GlobalSound : MonoBehaviour {

    AudioSource source;

	void Start () {
        source = GetComponent<AudioSource>();
	}

    public void Play(AudioClip clip)
    {
        source.Stop();
        source.clip = clip;
        source.Play();
    }
}
