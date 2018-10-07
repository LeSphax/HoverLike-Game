using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveFocusFromAbilities : SlideBall.MonoBehaviour, ISelectHandler, IDeselectHandler
{
    GUIPart previousPart;

    public void OnDeselect(BaseEventData eventData)
    {
        MyComponents.InputManager.currentPart = previousPart;
    }

    public void OnSelect(BaseEventData eventData)
    {
        previousPart = MyComponents.InputManager.currentPart;
        MyComponents.InputManager.currentPart = GUIPart.CHAT;
    }
}
