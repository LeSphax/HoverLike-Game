using UnityEngine;

public class MatchCountdown : Countdown
{

    [SerializeField]
    private GameObject readyButton;
    [SerializeField]
    private GameObject warmupInfo;

    private short syncId;
    private float timeToCallCloseToEnd = -1;

    public event EmptyEventHandler CloseToEnd;

    public override float TimeLeft
    {
        set
        {
            base.TimeLeft = value;
            if (base.TimeLeft < timeToCallCloseToEnd)
            {
                if (CloseToEnd != null)
                    CloseToEnd.Invoke();
                CloseToEnd = null;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        warmupInfo.SetActive(false);
        readyButton.SetActive(false);
    }

    public void RegisterCloseToEnd(EmptyEventHandler callback, float timeToCall, bool doRegister = true)
    {
        if (doRegister)
        {
            timeToCallCloseToEnd = timeToCall;
            CloseToEnd += callback;
        }
        else
        {
            CloseToEnd -= callback;
        }
    }

    //Called by MatchManager
    [MyRPC]
    private void StartWarmup(short syncId)
    {
        warmupInfo.SetActive(false);
        readyButton.SetActive(true);
        this.syncId = syncId;
        text.text = Language.Instance.texts["Warmup"];
    }

    //Called by Ready button
    public void StopWarmup()
    {
        readyButton.SetActive(false);
        warmupInfo.SetActive(true);
        MyComponents.PlayersSynchronisation.SendSynchronisation(syncId);
    }

    [MyRPC]
    protected void StartMatch(float timeLeft)
    {
        base.StartTimer(timeLeft);
        warmupInfo.SetActive(false);
    }
}
