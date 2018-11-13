using PlayerManagement;
using UnityEngine;

public abstract class AbilityInputWithBall : AbilityInput
{
    private bool hasBall = false;

    protected override void Start()
    {
        base.Start();
        MyComponents.Players.players[PlayerId].HasBallChanged += SetHasBall;
        MyComponents.Players.players[PlayerId].eventNotifier.ListenToEvents(PlayerStateChanged, PlayerFlags.STEALING_STATE);
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
        return base.CurrentStateAllowActivation(player) && MyComponents.Players.players[PlayerId].State.Stealing != StealingState.PROTECTED;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (MyComponents.Players.players[PlayerId] != null)
        {
            MyComponents.Players.players[PlayerId].HasBallChanged -= SetHasBall;
            MyComponents.Players.players[PlayerId].eventNotifier.StopListeningToEvents(PlayerStateChanged, PlayerFlags.STEALING_STATE);
        }
    }

}
