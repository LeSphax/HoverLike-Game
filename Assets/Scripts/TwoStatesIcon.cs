using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TwoStatesIcon : MonoBehaviour
{
    public Sprite Idle;
    public Sprite Activated;

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        SetActivated(false);
    }

    public void SetActivated(bool activated)
    {
        if (activated)
            image.sprite = Activated;
        else
            image.sprite = Idle;

    }
}