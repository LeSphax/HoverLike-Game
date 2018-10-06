using Byn.Net;
using SlideBall.Networking;

public abstract class ANetworkManagement : SlideBall.MonoBehaviour
{

    public abstract bool IsServer { get; }
    public abstract bool CurrentlyPlaying { get; set; }
    public abstract bool IsConnected { get; }
    public abstract void RefreshRoomData();
    public abstract int GetNumberPlayers();
    public abstract void Reset();

    public abstract event EmptyEventHandler ReceivedAllBufferedMessages;

    public abstract void SendData(short viewId, MessageType type, byte[] data);
    public abstract void SendData(short viewId, MessageType type, byte[] data, ConnectionId id);
    public abstract void SendData(short viewId, short subId, MessageType type, byte[] data);
    public abstract void SendData(short viewId, short subId, MessageType type, byte[] data, ConnectionId id);
    public abstract void SendNetworkMessage(NetworkMessage message);
    public abstract void SendNetworkMessage(NetworkMessage message, params ConnectionId[] connectionIds);
}
