using PlayerManagement;
using UnityEngine;

public abstract class AbilityInputWithBall : AbilityInput
{
    private bool hasBall = false;

    protected override void Start()
    {
        Players.MyPlayer.HasBallChanged += SetHasBall;
    }

    private void SetHasBall(bool hasBall)
    {
        bool previousActivation = IsActivated;
        this.hasBall = hasBall;

        if (previousActivation != IsActivated)
            InvokeCanBeActivatedChanged();
    }

    protected override bool CanBeActivated()
    {
        return hasBall;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (Players.MyPlayer != null)
            Players.MyPlayer.HasBallChanged -= SetHasBall;
    }

}
