using Byn.Net;

public abstract class TimeStrategy
{
    protected TimeManagement management;
    public event ConnectionEventHandler NewConnection;

    public TimeStrategy(TimeManagement management)
    {
        this.management = management;
    }

    internal abstract bool IsSendingPackets();
    internal abstract byte[] CreatePacket();
    public abstract void PacketReceived(ConnectionId id, byte[] data);
    public abstract float GetLatency(ConnectionId id);
    public abstract float GetMyLatency();
    internal abstract float GetNetworkTime();

    protected void InvokeNewConnection(ConnectionId id)
    {
            NewConnection.Invoke(id);
    }
}
