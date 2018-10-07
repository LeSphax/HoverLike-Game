using PlayerManagement;
using UnityEngine;

public abstract class AbilityInputWithBall : AbilityInput
{
    private bool hasBall = false;

    protected override void Start()
    {
        base.Start();
        MyComponents.Players.MyPlayer.HasBallChanged += SetHasBall;
        MyComponents.Players.MyPlayer.eventNotifier.ListenToEvents(PlayerStateChanged, PlayerFlags.STEALING_STATE);
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
        return base.CurrentStateAllowActivation(player) && MyComponents.Players.MyPlayer.State.Stealing != StealingState.PROTECTED;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (MyComponents.Players.MyPlayer != null)
        {
            MyComponents.Players.MyPlayer.HasBallChanged -= SetHasBall;
            MyComponents.Players.MyPlayer.eventNotifier.StopListeningToEvents(PlayerStateChanged, PlayerFlags.STEALING_STATE);
        }
    }

}
