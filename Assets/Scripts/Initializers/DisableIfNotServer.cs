using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class DisableIfNotServer : MonoBehaviour
{
    void Start()
    {
        if (!MyComponents.NetworkManagement.isServer)
            GetComponent<Selectable>().interactable = false;
    }
}
