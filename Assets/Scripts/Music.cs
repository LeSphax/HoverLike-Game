using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Music : MonoBehaviour
{

    public AudioSource source;
    public Dropdown dropdown;

    public AudioClip[] clips;

    bool pause = false;
    bool muted = false;

    const int dropdownNoMusic = 0;

    // Use this for initialization
    void Start()
    {
        List<Dropdown.OptionData> options = clips.Select(clip => new Dropdown.OptionData(clip.name)).ToList();
        options.Insert(0, new Dropdown.OptionData("No Music"));

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(ChangeMusic);

        MyComponents.GameState.MatchStartOrEnd += StartMusic;
    }

    private void StartMusic(bool started)
    {
        if (!muted)
        {
            if (started)
                dropdown.value = 1;
            else
            {
                dropdown.value = 0;
                muted = false;
            }
        }
    }

    private void Update()
    {
        if (!pause && dropdown.value != dropdownNoMusic && !source.isPlaying)
        {
            dropdown.value = (dropdown.value % (dropdown.options.Count - 1)) + 1;
        }
    }

    void ChangeMusic(int newValue)
    {
        Debug.Log("ChangeMusic " + newValue);
        if (newValue == dropdownNoMusic)
        {
            source.clip = null;
            source.Stop();
            muted = true;
        }
        else
        {
            source.clip = clips[newValue - 1];
            pause = true;
            Invoke("Play", 7);
            muted = false;
        }
    }

    void Play()
    {
        source.Play();
        pause = false;
    }
}
