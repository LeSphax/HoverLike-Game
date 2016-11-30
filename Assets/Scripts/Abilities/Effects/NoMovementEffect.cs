using PlayerManagement;

public class NoMovementEffect : AbilityEffect
{
    public float duration;

    public override void ApplyOnTarget(params object[] parameters)
    {
        Players.MyPlayer.CurrentState = Player.State.NO_MOVEMENT;
        Invoke("StopFreezing", duration);
    }

    private void StopFreezing()
    {
        if (Players.MyPlayer.CurrentState == Player.State.NO_MOVEMENT)
            Players.MyPlayer.CurrentState = Player.State.PLAYING;
    }
}
