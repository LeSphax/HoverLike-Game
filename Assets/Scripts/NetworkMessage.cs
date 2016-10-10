using System;


[Serializable]
public class NetworkMessage
{
    private const int MAX_NUMBER_VIEWS = 1000;
    private int id;
    public int viewId
    {
        get
        {
            return id % MAX_NUMBER_VIEWS;
        }
        set
        {
            id = value + subId;
        }
    }
    public int subId
    {
        get
        {
            return id / MAX_NUMBER_VIEWS;
        }
        set
        {
            id = viewId + value * MAX_NUMBER_VIEWS;
        }
    }
    public MessageType type;
    public MessageFlags flags = MessageFlags.None;
    public byte[] data;

    private NetworkMessage(int viewId, byte[] data)
    {
        this.viewId = viewId;
        this.data = data;
    }

    public NetworkMessage(int viewId, MessageType type, byte[] data) : this(viewId, data)
    {
        this.type = type;
        SetFlagsFromType();
    }

    public NetworkMessage(int viewId, int subId, MessageType type, byte[] data) : this(viewId, type, data)
    {
        this.subId = subId;
    }

    public NetworkMessage(int viewId, int subId, RPCTargets targets, byte[] data) : this(viewId, data)
    {
        this.subId = subId;
        type = MessageType.RPC;
        SetFlagsFromRPCTargets(targets);
    }

    private void SetFlagsFromRPCTargets(RPCTargets targets)
    {
        flags |= MessageFlags.Reliable;
        switch (targets)
        {
            case RPCTargets.All:
            case RPCTargets.Others:
            case RPCTargets.AllBuffered:
            case RPCTargets.OthersBuffered:
            case RPCTargets.AllViaServer:
            case RPCTargets.AllBufferedViaServer:
                flags |= MessageFlags.Distributed;
                break;
            case RPCTargets.Server:
                break;
            default:
                break;
        }
        switch (targets)
        {
            case RPCTargets.AllBuffered:
            case RPCTargets.OthersBuffered:
            case RPCTargets.AllBufferedViaServer:
                flags |= MessageFlags.Buffered;
                break;
            case RPCTargets.All:
            case RPCTargets.Others:
            case RPCTargets.AllViaServer:
            case RPCTargets.Server:
                break;
            default:
                break;
        }
        switch (targets)
        {
            case RPCTargets.AllBuffered:
            case RPCTargets.OthersBuffered:
            case RPCTargets.All:
            case RPCTargets.Server:
            case RPCTargets.Others:
                break;
            case RPCTargets.AllViaServer:
            case RPCTargets.AllBufferedViaServer:
                flags |= MessageFlags.SentBack;
                break;
            default:
                break;
        }
    }

    private void SetFlagsFromType()
    {
        if (IsTypeBuffered())
        {
            flags |= MessageFlags.Buffered;
        }
        if (IsTypeDistributed())
        {
            flags |= MessageFlags.Distributed;
        }
        if (IsTypeReliable())
        {
            flags |= MessageFlags.Reliable;
        }
        if (IsTypeSentBack())
        {
            flags |= MessageFlags.SentBack;
        }
    }

    public bool isReliable()
    {
        return (flags & MessageFlags.Reliable) != 0;
    }

    public bool isBuffered()
    {
        return (flags & MessageFlags.Buffered) != 0;
    }

    public bool isDistributed()
    {
        return (flags & MessageFlags.Distributed) != 0;
    }

    public bool isSentBack()
    {
        return (flags & MessageFlags.SentBack) != 0;
    }

    private bool IsTypeReliable()
    {
        switch (type)
        {
            case MessageType.Instantiate:
            case MessageType.Ping:
            case MessageType.Properties:
            case MessageType.RPC:
            case MessageType.StartGame:
                return true;
            case MessageType.ViewPacket:
                return false;
            default:
                throw new UnhandledSwitchCaseException(type);
        }
    }

    

    private bool IsTypeBuffered()
    {
        switch (type)
        {

            case MessageType.Instantiate:
                return true;
            case MessageType.Ping:
            case MessageType.RPC:
            case MessageType.ViewPacket:
            case MessageType.Properties:
            case MessageType.StartGame:
                return false;
            default:
                throw new UnhandledSwitchCaseException(type);
        }
    }

   

    private bool IsTypeDistributed()
    {
        switch (type)
        {

            case MessageType.Instantiate:
            case MessageType.RPC:
            case MessageType.ViewPacket:
            case MessageType.Properties:
                return true;
            case MessageType.Ping:
            case MessageType.StartGame:
                return false;
            default:
                throw new UnhandledSwitchCaseException(type);
        }
    }

    private bool IsTypeSentBack()
    {
        switch (type)
        {
            case MessageType.Instantiate:
            case MessageType.RPC:
            case MessageType.ViewPacket:
            case MessageType.Properties:
            case MessageType.Ping:
            case MessageType.StartGame:
                return false;
            default:
                throw new UnhandledSwitchCaseException(type);
        }
    }

    public override string ToString()
    {
        return "Id : " + viewId + "-" + subId + ", type : " + type + " flags : " + flags;
    }
}

public enum MessageType
{
    ViewPacket,
    Instantiate,
    Properties,
    Ping,
    RPC,
    StartGame,
}

[Flags]
public enum MessageFlags
{
    None = 0,
    Reliable = 1,
    Distributed = 2,
    Buffered = 4,
    SentBack = 8,
}


