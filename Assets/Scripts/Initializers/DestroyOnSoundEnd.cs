using UnityEngine;

public class DestroyOnSoundEnd : MonoBehaviour {

    private AudioSource source;
    bool checkDelete = false;

	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
        //Seems like it will be destroyed instantly on WebGL if the check is not delayed a bit. 
        Invoke("StartCheckDelete", 0.5f);
	}
	
	// Update is called once per frame
	void Update () {
        if (checkDelete && !source.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    private void StartCheckDelete()
    {
        checkDelete = true;
    }
}
