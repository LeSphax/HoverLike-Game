using PlayerBallControl;
using PlayerManagement;
using UnityEngine;

public class AIRandomMovement : MonoBehaviour
{
    JumpEffect jumpEffect;
    MoveEffect moveEffect;
    private float MoveDelay = 3f;
    private float JumpDelay = 10f;

    // Use this for initialization
    void Start()
    {
        jumpEffect = gameObject.AddComponent<JumpEffect>();
        moveEffect = gameObject.AddComponent<MoveEffect>();
        InvokeMove();
        InvokeJump();
    }

    private void Move()
    {
        Vector3 target = Functions.GetRandomPointInVolume(Vector3.zero, new Vector3(-60, 5, -140), new Vector3(60, 5, 140));
        moveEffect.ApplyOnTarget(Players.MyPlayer.controller, target);
        InvokeMove();
    }

    private void InvokeMove()
    {
        Invoke("Move", Random.Range(MoveDelay, MoveDelay * 2));
    }

    private void Jump()
    {
        jumpEffect.ApplyOnTarget(Players.MyPlayer.controller);
        InvokeJump();
    }

    private void InvokeJump()
    {
        Invoke("Jump", Random.Range(JumpDelay, JumpDelay * 2));
    }
}
