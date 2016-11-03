using UnityEngine;
using UnityEngine.UI;

public class Countdown : SlideBall.MonoBehaviour
{
    [SerializeField]
    private Text display;
    [SerializeField]
    private Text title;

    public event EmptyEventHandler TimerFinished;

    public string Title
    {
        set
        {
            title.text = value;
        }
        get
        {
            return title.text;
        }
    }
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
                display.text = "";
            }
            else
            {
                display.text = "" + (int)timeLeft;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeLeft > 0)
        {
            TimeLeft -= Time.deltaTime;
            if (TimeLeft <= 0)
            {
                Title = "";
                if (TimerFinished != null)
                    TimerFinished.Invoke();
            }
        }
    }

    [MyRPC]
    private void StartTimer(string title, float timeLeft)
    {
        TimeLeft = timeLeft - TimeManagement.LatencyInMiliseconds / 2000f;
        Title = title;
    }
}
