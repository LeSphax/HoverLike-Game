using CustomAnimations;
using UnityEngine;

public class DestroyAfterTimeout : MonoBehaviour
{

    public float timeout;
    public MyAnimation m_animation;

    void Start()
    {
        if (m_animation != null)
        {
            Invoke("StartDestruction", timeout - m_animation.duration);

        }
        else
            Invoke("Destroy", timeout);
    }

    void StartDestruction()
    {
        m_animation.FinishedAnimating += Destroy;
        Debug.Log(m_animation.IsCompleted);
        if (m_animation.IsCompleted)
            m_animation.StartReverseAnimating();
        else
            m_animation.StartAnimating();
    }

    void Destroy(MyAnimation animation = null)
    {
        Destroy(gameObject);
    }
}
