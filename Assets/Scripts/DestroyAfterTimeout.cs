using UnityEngine;

public class DestroyAfterTimeout : MonoBehaviour {

    public float timeout;

	void Start () {
        Invoke("DestroyThis", timeout);
	}

    void DestroyThis()
    {
        Destroy(gameObject);
    }
}
