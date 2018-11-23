using UnityEngine;

public class TrainingRoom : SlideBall.MonoBehaviour
{

    public GameObject top;
    public GameObject bottom;
    public GameObject right;
    public GameObject left;

    private readonly Goal[] mGoals = new Goal[2];

    public void Start()
    {
        AcademySB.AcademyResetEvent += Reset;

        Goal[] goals = MyComponents.GetComponentsInChildren<Goal>();
        foreach (Goal goal in goals)
        {
            mGoals[goal.teamNumber] = goal;
        }

        Reset();

    }

    public void Reset()
    {
        float zPos = Mathf.Max(AcademySB.maxZ + 10, 155);
        float xPos = AcademySB.maxX + 10;
        bottom.transform.localPosition = new Vector3(0f, 1f, zPos);
        top.transform.localPosition = new Vector3(0f, 1f, -zPos);
        right.transform.localPosition = new Vector3(xPos, 1f, 0f);
        left.transform.localPosition = new Vector3(-xPos, 1f, 0f);

        if (AcademySB.HasGoals)
        {
            float angle = Random.Range(-Mathf.PI, Mathf.PI);
            if (AcademySB.randomGoals)
                mGoals[0].transform.localPosition = Functions.GetPointOnEllipse(angle, AcademySB.maxX, AcademySB.maxZ);
            else
                mGoals[0].transform.localPosition = new Vector3(0f, 0f, -AcademySB.maxZ);
            mGoals[1].transform.localPosition = -mGoals[0].transform.localPosition;

            foreach (Goal goal in mGoals)
            {
                goal.transform.LookAt(transform.position);
                goal.SetSize(AcademySB.goalSize);
                goal.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Goal goal in mGoals)
            {
                goal.gameObject.SetActive(false);
            }
        }
    }

}
