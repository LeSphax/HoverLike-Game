using UnityEngine;

public class WarmupCountdown : Countdown
{

    [SerializeField]
    private GameObject readyButton;
    [SerializeField]
    private GameObject warmupInfo;

    private short syncId;

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
        text.text = "Warmup";
    }

    //Called by Ready button
    public void StopWarmup()
    {
        readyButton.SetActive(false);
        warmupInfo.SetActive(true);
        MyComponents.PlayersSynchronisation.SendSynchronisation(syncId);
    }

    [MyRPC]
    protected override void StartTimer(float timeLeft)
    {
        base.StartTimer(timeLeft);
        warmupInfo.SetActive(false);
    }
}
