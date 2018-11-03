using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiator : MonoBehaviour
{


    public int nbX;
    public int nbY;
    public float XSpace;
    public float ZSpace;

    public bool onlyOne;

    public GameObject arenaPrefab;

    private List<GameObject> arenas = new List<GameObject>();

    private void Start()
    {
#if UNITY_EDITOR
        if (onlyOne)
        {
            nbX = 1;
            nbY = 1;
        }
#endif
        Reset();
    }

    private void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            Reset();
        }
    }

    private void Reset()
    {
        arenas.ForEach(gym => Destroy(gym));
        arenas.Clear();

        for (int i = 1; i <= nbX; i++)
        {
            for (int j = 1; j <= nbY; j++)
            {
                GameObject arena = Instantiate(arenaPrefab, transform);
                //foreach (AgentCatchBall agent in gym.GetComponentsInChildren<AgentCatchBall>())
                //{
                //    agent.GiveBrain(brain.GetComponent<Brain>());
                //}
                arena.transform.localPosition = new Vector3(i * XSpace, 0, j * ZSpace);
                arenas.Add(arena);
            }
        }
    }


}


