using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AnimatedGIF : MonoBehaviour
{
    private List<Sprite> frames = new List<Sprite>();
    public int framesPerSecond = 15;
    private int rate = 2;

    public int activateIconStart;
    public int activateIconEnd;

    public int startIndex;
    public int endIndex;

    public TwoStatesIcon Icon;

    public string path;

    private void Start()
    {
        int currentFrame = startIndex;
        string s_currentFrame = currentFrame.ToString();
        while (s_currentFrame.Length < 3)
        {
            s_currentFrame = "0" + s_currentFrame;
        }
        Sprite frame = Resources.Load<Sprite>(path + s_currentFrame);
        Debug.Log(path + s_currentFrame);
        while (currentFrame <= endIndex)
        {
            frames.Add(frame);
            currentFrame += rate;
            s_currentFrame = currentFrame.ToString();
            while (s_currentFrame.Length < 3)
            {
                s_currentFrame = "0" + s_currentFrame;
            }
            frame = Resources.Load<Sprite>(path + s_currentFrame);
        }
    }

    void Update()
    {
        int index = (int)(Time.time * framesPerSecond) % frames.Count;
        if (Icon != null)
            ActivateIcon(index * rate);
        GetComponent<Image>().sprite = frames[index];
    }

    private void ActivateIcon(int index)
    {
        if (activateIconStart >= 0)
            if (index >= activateIconStart && index <= activateIconEnd)
            {
                Icon.SetActivated(true);
            }
            else
                Icon.SetActivated(false);
    }
}
