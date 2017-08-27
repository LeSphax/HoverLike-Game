using UnityEngine;

public class GUIInputs : MonoBehaviour
{
    public MatchPanel matchPanel;

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            matchPanel.Open(!matchPanel.gameObject.activeSelf);
        }
    }
}
