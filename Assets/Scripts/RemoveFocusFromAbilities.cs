using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveFocusFromAbilities : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    GUIPart previousPart;

    public void OnDeselect(BaseEventData eventData)
    {
        SlideBallInputs.currentPart = previousPart;
    }

    public void OnSelect(BaseEventData eventData)
    {
        previousPart = SlideBallInputs.currentPart;
        SlideBallInputs.currentPart = GUIPart.CHAT;
    }
}
