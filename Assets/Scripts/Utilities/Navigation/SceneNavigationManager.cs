using UnityEngine;

namespace Navigation
{
    public class SceneNavigationManager : MonoBehaviour
    {
        protected bool readyToReturn = true;
        protected virtual void Update()
        {
            if (readyToReturn && Input.GetButtonDown(InputButtonNames.RETURN))
            {
                GetComponent<NavigationManager>().GoBack();
            }
        }
    }
}
