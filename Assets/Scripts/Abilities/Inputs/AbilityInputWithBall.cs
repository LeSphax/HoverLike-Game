using PlayerManagement;
using UnityEngine;

public abstract class AbilityInputWithBall : AbilityInput
{
    private bool hasBall = false;

    protected override void Start()
    {
        base.Start();
        Players.MyPlayer.HasBallChanged += SetHasBall;
        Players.MyPlayer.eventNotifier.ListenToEvents(PlayerStateChanged, PlayerFlags.STEALING_STATE);
    }

    private void SetHasBall(bool hasBall)
    {
        bool previousActivation = IsActivated;
        this.hasBall = hasBall;

        if (previousActivation != IsActivated)
            InvokeCanBeActivatedChanged();
    }

    protected override bool AdditionalActivationRequirementsAreMet()
    {
        return hasBall;
    }

    protected override bool CurrentStateAllowActivation(Player player)
    {
        return base.CurrentStateAllowActivation(player) && Players.MyPlayer.State.Stealing != StealingState.PROTECTED;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (Players.MyPlayer != null)
            Players.MyPlayer.HasBallChanged -= SetHasBall;
    }

}
