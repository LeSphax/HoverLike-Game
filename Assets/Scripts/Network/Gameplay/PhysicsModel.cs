using UnityEngine;

public abstract class PhysicsModel : MonoBehaviour
{
    [SerializeField]
    private MyNetworkView myNetworkView;



    protected virtual void Awake()
    {
        MyComponents.GameInitialization.AddGameStartedListener(Register);
    }

    private void Register()
    {
        Debug.Log("Register Physics Model " + this + "   " + myNetworkView.ViewId);
        MyComponents.PhysicsModelsManager.RegisterView(myNetworkView.ViewId, this);
    }

    public abstract void Simulate(float dt);

    public abstract byte[] Serialize();

    public abstract int DeserializeAndRewind(byte[] data, int offset);

    public abstract void CheckForPostSimulationActions();
    public abstract void CheckForPreSimulationActions();

    protected void OnDestroy()
    {
        MyComponents.PhysicsModelsManager.UnregisterView(myNetworkView.ViewId);
    }
}