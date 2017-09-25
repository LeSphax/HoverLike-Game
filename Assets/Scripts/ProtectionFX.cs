using UnityEngine;

public class ProtectionFX : MonoBehaviour {

    public void ShowProtection()
    {
        gameObject.SetActive(true);
        Invoke("Desactivate", StealEffect.protectionDuration);
    }

    void Desactivate()
    {
        gameObject.SetActive(false);
    }
}
