using UnityEngine;
using System.Collections;

public class TestAnimator : MonoBehaviour {
private bool Catch;
	
	// Update is called once per frame
	void Update () {
	if (Input.GetKeyDown(KeyCode.Space))
        {
            Catch = !Catch;
            GetComponent<Animator>().SetBool("Catch",Catch);
        }
	}
}
