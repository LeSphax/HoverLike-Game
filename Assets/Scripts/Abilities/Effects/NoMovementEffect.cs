using PlayerManagement;

public class NoMovementEffect : AbilityEffect
{
    public float duration;

    public override void ApplyOnTarget(params object[] parameters)
    {
        MyComponents.MyPlayer.State.Movement = MovementState.NO_MOVEMENT;
        Invoke("StopFreezing", duration);
    }

    private void StopFreezing()
    {
        if  (MyComponents.MyPlayer.State.Movement == MovementState.NO_MOVEMENT)
            MyComponents.MyPlayer.State.Movement = MovementState.PLAYING;
    }
}
