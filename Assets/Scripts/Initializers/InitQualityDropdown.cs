using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class InitQualityDropdown : MonoBehaviour {

    Dropdown dropdown;

    private void Start()
    {
        dropdown = GetComponent<Dropdown>();
        string[] names = QualitySettings.names;
        dropdown.AddOptions(new List<string>(names));
        dropdown.value = QualitySettings.GetQualityLevel();

        dropdown.onValueChanged.AddListener(SetQuality);
    }

    private void SetQuality(int level)
    {
        QualitySettings.SetQualityLevel(level, true);
        Debug.Log("Set quality to level : " + QualitySettings.names[level]);
    }
}