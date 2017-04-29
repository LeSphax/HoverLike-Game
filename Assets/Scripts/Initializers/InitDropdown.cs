using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class InitDropdown : MonoBehaviour {

    private void Start()
    {
        GetComponent<Dropdown>().value = 7;
    }
}