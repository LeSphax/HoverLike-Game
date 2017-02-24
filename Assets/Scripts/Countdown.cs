using UnityEngine;
using UnityEngine.UI;

public class Countdown : SlideBall.MonoBehaviour
{
    [SerializeField]
    protected Text text;

    protected string endMessage;

    protected AudioSource audioSource;

    public AudioClip tickSound;
    public AudioClip entrySound;
    public AudioClip matchEndSound;

    public event EmptyEventHandler TimerFinished;

    private bool paused;

    [SerializeField]
    private float timeLeft = -1;
    public virtual float TimeLeft
    {
        get
        {
            return timeLeft;
        }
        set
        {
            timeLeft = value;
            if (timeLeft <= -1)
            {
                text.text = "";
            }
            else
            {
                if (shownNumber == 0)
                {
                    text.text = endMessage;
                }
                else if (shownNumber > 60)
                    if (shownNumber % 60 >= 10)
                        text.text = "" + shownNumber / 60 + ":" + shownNumber % 60;
                    else
                        text.text = "" + shownNumber / 60 + ":0" + shownNumber % 60;
                else
                    text.text = "" + shownNumber;
            }
        }
    }

    // It's better to change the number to the new integer when it actually reaches that integer-> 1,598 show 2 and switches to 1 when there is exactly 1 second left. 
    private int shownNumber
    {
        get
        {
            if (timeLeft > 0)
                return (int)timeLeft + 1;
            else
                return 0;
        }
    }

    protected virtual void Awake()
    {
        text.text = "";
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeLeft > -1 && !paused)
        {
            int integer = shownNumber;
            TimeLeft -= Time.deltaTime;
            //Because shownNumber is timeLeft+1, we avoid the problem of (int)0.5 = (int)-0.7
            if (integer != shownNumber)
            {
                if (shownNumber > 0)
                    PlayTickSound();
                else if (shownNumber == 0)
                    PlayEntrySound();
            }

            if (TimeLeft <= 0)
            {
                if (TimerFinished != null)
                {
                    TimerFinished.Invoke();
                    TimerFinished = null;
                }
            }
        }
    }

    [MyRPC]
    protected virtual void StartTimer(float timeLeft, string endMessage)
    {
        TimeLeft = timeLeft - TimeManagement.LatencyInMiliseconds / 2000f;
        this.endMessage = endMessage;
    }

    public virtual void PauseTimer(bool pause)
    {
        if (paused != pause)
            View.RPC("RPCPauseTimer", RPCTargets.All, pause);
    }

    [MyRPC]
    protected virtual void RPCPauseTimer(bool pause)
    {
        paused = pause;
    }

    [MyRPC]
    protected void StopTimer()
    {
        TimerFinished = null;
        TimeLeft = 0;
    }

    [MyRPC]
    public void SetText(string newText)
    {
        text.text = newText;
    }

    private void PlayEntrySound()
    {
        if (audioSource != null)
        {
            audioSource.clip = entrySound;
            audioSource.Play();
        }
    }
    private void PlayTickSound()
    {
        if (audioSource != null)
        {
            audioSource.clip = tickSound;
            audioSource.Play();
        }
    }

    public void PlayMatchEndSound()
    {
        if (audioSource != null)
        {
            audioSource.clip = matchEndSound;
            audioSource.Play();
        }
    }
}
