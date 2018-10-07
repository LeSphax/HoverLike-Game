using PlayerManagement;
using UnityEngine;

public class BallThrower : SlideBall.MonoBehaviour
{
    public GameObject goal;
    public float timeToWaitBeforeThrow = 0f;
    public float timeToWaitBeforeThrowMax = 0f;

    // Use this for initialization
    void Start()
    {
#if !UNITY_EDITOR
        Destroy(goal);
        Destroy(gameObject);
        EditorVariables.CanScoreGoals = true;
#else
        InvokeRepeating("ChangeBallPosition", 1f, 2f);
        EditorVariables.CanScoreGoals = false;
#endif
    }

    // Update is called once per frame
    void ChangeBallPosition()
    {
        MyComponents.BallState.DetachBall();
        MyComponents.BallState.GetComponent<Rigidbody>().velocity = Vector3.zero;
        MyComponents.BallState.transform.position = Functions.GetRandomPointInCube(gameObject);
        float timeToWait = Random.Range(timeToWaitBeforeThrow, timeToWaitBeforeThrowMax);
        Invoke("ThrowBall", timeToWait);
    }

    private void ThrowBall()
    {
        MyComponents.BallState.GetComponent<BallMovementView>().Throw(Functions.GetRandomPointInCube(goal), 1);
    }
}
