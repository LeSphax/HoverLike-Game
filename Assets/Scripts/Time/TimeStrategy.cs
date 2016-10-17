using Byn.Net;

public abstract class TimeStrategy
{
    protected TimeManagement management;

    public TimeStrategy(TimeManagement management)
    {
        this.management = management;
    }

    internal abstract bool IsSendingPackets();
    internal abstract byte[] CreatePacket();
    public abstract void PacketReceived(ConnectionId id, byte[] data);
    public abstract float GetLatency(ConnectionId id);
    internal abstract float GetNetworkTime();
}
