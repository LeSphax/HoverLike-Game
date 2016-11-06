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
    public abstract float GetLatencyInMiliseconds(ConnectionId id);
    public abstract float GetMyLatencyInMiliseconds();
    internal abstract float GetNetworkTimeInSeconds();

    protected void InvokeNewConnection(ConnectionId id)
    {
            NewConnection.Invoke(id);
    }
}
