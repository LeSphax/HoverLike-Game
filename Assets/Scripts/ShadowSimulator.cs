using UnityEngine;

public class ShadowSimulator : MonoBehaviour
{

    private GameObject shadowPrefab;
    private GameObject shadow;
    private float groundLevel;

    public float size;

    // Use this for initialization
    void Start()
    {
        shadowPrefab = Resources.Load<GameObject>(Paths.SHADOW_PREFAB);
        shadow = Instantiate<GameObject>(shadowPrefab);
        shadow.transform.localScale *= size;
        GameObject ground = GameObject.FindGameObjectWithTag(Tags.Ground);
        groundLevel = ground.transform.position.y + ground.transform.localScale.y / 2 + 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        shadow.transform.position = transform.position - transform.position.y * Vector3.up + Vector3.up * groundLevel;
    }

    private void OnDestroy()
    {
        Destroy(shadow);
    }
}
