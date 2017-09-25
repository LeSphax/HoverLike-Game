using PlayerManagement;

public class NoMovementEffect : AbilityEffect
{
    public float duration;

    public override void ApplyOnTarget(params object[] parameters)
    {
        Players.MyPlayer.State.Movement = MovementState.NO_MOVEMENT;
        Invoke("StopFreezing", duration);
    }

    private void StopFreezing()
    {
        if (Players.MyPlayer.State.Movement == MovementState.NO_MOVEMENT)
            Players.MyPlayer.State.Movement = MovementState.PLAYING;
    }
}
