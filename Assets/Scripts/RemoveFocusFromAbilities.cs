using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveFocusFromAbilities : SlideBall.MonoBehaviour, ISelectHandler, IDeselectHandler
{
    GUIPart previousPart;

    public void OnDeselect(BaseEventData eventData)
    {
        InputManager.currentPart = previousPart;
    }

    public void OnSelect(BaseEventData eventData)
    {
        previousPart = InputManager.currentPart;
        InputManager.currentPart = GUIPart.CHAT;
    }
}
