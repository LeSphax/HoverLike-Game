using UnityEngine;
using UnityEngine.UI;

public class Countdown : SlideBall.MonoBehaviour
{
    [SerializeField]
    protected Text text;


    public event EmptyEventHandler TimerFinished;

    [SerializeField]
    private float timeLeft = 0;
    public float TimeLeft
    {
        private get
        {
            return timeLeft;
        }
        set
        {
            timeLeft = value;
            if (timeLeft <= 0)
            {
                text.text = "";
            }
            else
            {
                if (timeLeft > 60)
                    if ((int)timeLeft % 60 >= 10)
                        text.text = "" + (int)timeLeft / 60 + ":" + (int)timeLeft % 60;
                    else
                        text.text = "" + (int)timeLeft / 60 + ":0" + (int)timeLeft % 60;
                else
                    text.text = "" + (int)timeLeft;
            }
        }
    }

    protected virtual void Awake()
    {
        text.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            if (TimeLeft <= 0)
            {
                if (TimerFinished != null)
                    TimerFinished.Invoke();
            }
        }
    }

    [MyRPC]
    protected virtual void StartTimer(float timeLeft)
    {
        TimeLeft = timeLeft - TimeManagement.LatencyInMiliseconds / 2000f;
    }

    [MyRPC]
    private void StopTimer()
    {
        TimerFinished = null;
        TimeLeft = 0;
    }

    [MyRPC]
    public void SetText(string newText)
    {
        text.text = newText;
    }

    //For debugging
    public void SetTimeLeft(int timeLeft)
    {
        TimeLeft = timeLeft;
    }
}
