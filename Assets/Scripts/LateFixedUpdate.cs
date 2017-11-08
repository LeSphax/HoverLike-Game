using UnityEngine;
using UnityEngine.SceneManagement;

public class LateFixedUpdate : MonoBehaviour
{

    public static event EmptyEventHandler Evt;

    bool fixedUpdate;
    Collider m_collider;
    GameObject other;

    private void Start()
    {
        Reset();
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Reset();
    }

    private void Reset()
    {
        if (m_collider != null)
            Destroy(m_collider);
        if (other != null)
            Destroy(other);


        m_collider = gameObject.AddComponent<BoxCollider>();
        m_collider.isTrigger = true;

        other = new GameObject();
        other.AddComponent<BoxCollider>().isTrigger = true;
        other.AddComponent<Rigidbody>().isKinematic = true;
        other.transform.parent = transform;
        other.transform.position = transform.position + Vector3.up * 0.1f;
        other.name = "LateFixedUpdateHack";
    }

    void OnL()
    {
        // Create a couple of colliders to trigger OnTriggerStay each fixed update frame after the physics run

    }

    void FixedUpdate()
    {
        fixedUpdate = true;
    }

    void OnTriggerStay()
    {
        // Multiple OnTriggerStay could be called if there is another collider
        if (!fixedUpdate) return;
        fixedUpdate = false;
        if (Evt != null) Evt();
    }
}
