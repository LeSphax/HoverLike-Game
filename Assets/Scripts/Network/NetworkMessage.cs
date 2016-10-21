using System;


[Serializable]
public class NetworkMessage
{
    private const int MAX_NUMBER_VIEWS = Int16.MaxValue;
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

    public bool traceMessage = false;

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
                break;
            case RPCTargets.Specified:
            case RPCTargets.Server:
                flags |= MessageFlags.NotDistributed;
                break;
            default:
                break;
        }
        switch (targets)
        {
            case RPCTargets.AllBuffered:
            case RPCTargets.OthersBuffered:
                flags |= MessageFlags.Buffered;
                break;
            case RPCTargets.All:
            case RPCTargets.Others:
            case RPCTargets.Specified:
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
            case RPCTargets.Specified:
            case RPCTargets.Others:
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
        if (IsTypeNotDistributed())
        {
            flags |= MessageFlags.NotDistributed;
        }
        if (IsTypeReliable())
        {
            flags |= MessageFlags.Reliable;
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
        return (flags & MessageFlags.NotDistributed) == 0;
    }

    private bool IsTypeReliable()
    {
        switch (type)
        {
            case MessageType.Properties:
            case MessageType.RPC:
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
            case MessageType.RPC:
            case MessageType.ViewPacket:
            case MessageType.Properties:
                return false;
            default:
                throw new UnhandledSwitchCaseException(type);
        }
    }



    private bool IsTypeNotDistributed()
    {
        switch (type)
        {

            case MessageType.RPC:
            case MessageType.ViewPacket:
            case MessageType.Properties:
                return false;
            default:
                throw new UnhandledSwitchCaseException(type);
        }
    }

    private bool IsTypeSentBack()
    {
        switch (type)
        {
            case MessageType.RPC:
            case MessageType.ViewPacket:
            case MessageType.Properties:
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
    Properties,
    RPC,
}

[Flags]
public enum MessageFlags
{
    None = 0,
    Reliable = 1,
    NotDistributed = 2,
    Buffered = 4,
}


