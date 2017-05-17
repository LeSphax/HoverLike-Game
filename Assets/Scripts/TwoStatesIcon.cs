using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(Image))]
public class TwoStatesIcon : MonoBehaviour
{
    public Sprite Idle;
    public Sprite Activated;

    //private VideoPlayer video;

    //public float activateTime;
    //public float desactivateTime;

    private Image image;
    //private float videoStartTime;

    //public void SetVideoPlayer(VideoPlayer player)
    //{
    //    video = player;
    //    video.started += Restarting;
    //    video.loopPointReached += Restarting;
    //}

    //private float videoTime
    //{
    //    get
    //    {
    //        return Time.realtimeSinceStartup - videoStartTime;
    //    }
    //}

    private void Start()
    {
        image = GetComponent<Image>();
        image.sprite = Idle;
    }

    //private void Restarting(VideoPlayer source)
    //{
    //    videoStartTime = Time.realtimeSinceStartup;
    //}

    //private void Update()
    //{
    //    if (videoTime > activateTime && videoTime < desactivateTime && image.sprite == Idle)
    //    {
    //        image.sprite = Activated;
    //    }
    //    else if ((videoTime < activateTime || videoTime > desactivateTime) && image.sprite == Activated)
    //        image.sprite = Idle;
    //}

    public void SetActivated(bool activated)
    {
        if (activated && image.sprite == Idle)
            image.sprite = Activated;
        else if (!activated && image.sprite == Activated)
            image.sprite = Idle;

    }
}