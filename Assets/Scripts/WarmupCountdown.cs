using UnityEngine;

public class WarmupCountdown : Countdown
{

    [SerializeField]
    private GameObject readyButton;
    [SerializeField]
    private GameObject warmupInfo;
    [SerializeField]
    private Countdown matchEndCountdown;

    private short syncId;
    private int matchEndCountdownDuration = -1;

    public override float TimeLeft
    {
        set
        {
            base.TimeLeft = value;
            if ((int)base.TimeLeft == matchEndCountdownDuration)
            {
                matchEndCountdownDuration = -1;
                matchEndCountdown.StartTimer(base.TimeLeft,"");
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        warmupInfo.SetActive(false);
        readyButton.SetActive(false);
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
    protected void StartMatch(float timeLeft, int matchEndCountdownDuration)
    {
        base.StartTimer(timeLeft, "0");
        this.matchEndCountdownDuration = matchEndCountdownDuration;
        warmupInfo.SetActive(false);
    }
}
