using UnityEngine;

public class TrainingRoom : SlideBall.MonoBehaviour {

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
        float zPos = AcademySB.maxZ + 10;
        float xPos = AcademySB.maxX + 10;
        bottom.transform.localPosition = new Vector3(0f, 1f, zPos);
        top.transform.localPosition = new Vector3(0f, 1f, -zPos);
        right.transform.localPosition = new Vector3(xPos, 1f, 0f);
        left.transform.localPosition = new Vector3(-xPos, 1f, 0f);


        if (AcademySB.HasGoals)
        {
            mGoals[0].transform.localPosition = new Vector3(0f, 0f, -AcademySB.maxZ);
            mGoals[1].transform.localPosition = new Vector3(0f, 0f, AcademySB.maxZ);

            foreach (Goal goal in mGoals)
            {
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
