using UnityEngine;

public class CustomRigidbody : MonoBehaviour {

    [HideInInspector]
    public Vector3 velocity;
    public float drag;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position += velocity * Time.deltaTime;
        velocity = velocity * (1 - Time.deltaTime * drag);
    }

    public void AddForce(Vector3 force)
    {
        velocity += force;
    }
}
