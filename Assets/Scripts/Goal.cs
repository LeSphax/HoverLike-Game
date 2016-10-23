using UnityEngine;

public class Goal : MonoBehaviour
{

    public int teamNumber = 1;
    private AudioSource myAudio;
    private AudioSource Audio
    {
        get
        {
            if (myAudio == null)
            {
                myAudio = GetComponent<AudioSource>();
                AudioClip but = Resources.Load<AudioClip>("Audio/But");
                myAudio.clip = but;
            }
            return myAudio;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == Tags.Ball)
        {
            MyGameObjects.MatchManager.TeamScored(teamNumber);
            Debug.Log("But");
            Audio.Play();
        }
    }



}
