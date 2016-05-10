using UnityEngine;

class MatchManager : MonoBehaviour
{

    void Awake()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            gameObject.SetActive(false);
        }
    }


    public void AddPlayer()
    {

    }

    public void Goal(int teamNumber)
    {
        Invoke("StartNewPoint", 3f);

    }

    public void StartNewPoint()
    {

    }
}

