using UnityEngine;
using UnityEngine.UI;

public class Countdown : SlideBall.MonoBehaviour
{
    [SerializeField]
    protected Text text;

    protected AudioSource audioSource;

    public AudioClip tickSound;
    public AudioClip entrySound;
    public AudioClip matchEndSound;

    public event EmptyEventHandler TimerFinished;

    protected bool paused = true;

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
            if (ShownNumber < 0)
            {
                text.text = "";
            }
            else
            {
                if (ShownNumber > 60)
                    if (ShownNumber % 60 >= 10)
                        text.text = "" + ShownNumber / 60 + ":" + ShownNumber % 60;
                    else
                        text.text = "" + ShownNumber / 60 + ":0" + ShownNumber % 60;
                else
                    text.text = "" + ShownNumber;
            }
        }
    }

    // It's better to change the number to the new integer when it actually reaches that integer-> 1,598 show 2 and switches to 1 when there is exactly 1 second left. 
    private int ShownNumber
    {
        get
        {
            if (timeLeft > 0)
                if (timeLeft == (int)timeLeft)
                    return (int)timeLeft;
                else
                    return (int)timeLeft + 1;
            else
            {
                return (int)timeLeft;
            }
        }
    }

    protected virtual void Awake()
    {
        StopTimerAndSetText("");

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            int integer = ShownNumber;
            TimeLeft -= Time.deltaTime;
            //Because shownNumber is timeLeft+1, we avoid the problem of (int)0.5 = (int)-0.7
            if (integer != ShownNumber)
            {
                if (ShownNumber > -1)
                    PlayTickSound();
                    
            }
            if (TimeLeft <= -1)
            {
                PlayEntrySound();
                StopTimer();
                if (TimerFinished != null)
                {
                    TimerFinished.Invoke();
                }
            }
        }
    }

    [MyRPC]
    public virtual void StartTimer(float timeLeft)
    {
        TimeLeft = timeLeft - TimeManagement.LatencyInMiliseconds / 2000f;
    }

    public virtual void PauseTimer(bool pause)
    {
        if (paused != pause)
            View.RPC("RPCPauseTimer", RPCTargets.All, pause);
    }

    [MyRPC]
    protected virtual void RPCPauseTimer(bool pause)
    {
        if (pause != paused)
        {
            paused = pause;

            if (paused)
                TimeLeft = timeLeft + TimeManagement.LatencyInMiliseconds / 2000f;
            else
                TimeLeft = timeLeft - TimeManagement.LatencyInMiliseconds / 2000f;
        }
    }

    [MyRPC]
    protected void StopTimer()
    {
        TimeLeft = -1;
        paused = true;
    }

    [MyRPC]
    public void StopTimerAndSetText(string newText)
    {
        Debug.Log("StopTimer " + gameObject.name+ "  " + newText);
        TimeLeft = -1;
        paused = true;
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
        View.RPC("RPCMatchEndSound", RPCTargets.All);
    }

    [MyRPC]
    public void RPCMatchEndSound()
    {
        if (audioSource != null)
        {
            audioSource.clip = matchEndSound;
            audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        TimerFinished = null;
    }
}
