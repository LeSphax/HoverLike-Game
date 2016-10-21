

using UnityEngine;
using UnityEngine.UI;

public abstract class AbilityInput : MonoBehaviour
{
    public abstract bool Activated();
    public abstract string GetKey();

    void Start()
    {
        GameObject keyToUsePrefab = Resources.Load<GameObject>("Prefabs/KeyToUse");
        GameObject keyToUse = Instantiate(keyToUsePrefab);
        keyToUse.transform.SetParent(transform, false);
        keyToUse.GetComponentInChildren<Text>().text = GetKey();
    }
}