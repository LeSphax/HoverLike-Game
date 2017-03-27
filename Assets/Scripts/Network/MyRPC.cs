using SlideBall.Networking;
using System;

public class MyRPC : Attribute
{


}

public enum RPCTargets
{
    /// <summary>Sends the RPC to everyone else and executes it immediately on this client. Player who join later will not execute this RPC.</summary>
    All,
    /// <summary>Sends the RPC to everyone else. This client does not execute the RPC. Player who join later will not execute this RPC.</summary>
    Others,
    /// <summary>Sends the RPC to a specific player. This client does not execute the RPC. Player who join later will not execute this RPC.</summary>
    Specified,
    /// <summary>Sends the RPC to MasterClient only. Careful: The MasterClient might disconnect before it executes the RPC and that might cause dropped RPCs. If the sender is the server the RPC will be executed in place</summary>
    Server,
    /// <summary>Sends the RPC to everyone else and executes it immediately on this client. New players get the RPC when they join as it's buffered (until this client leaves).</summary>
    AllBuffered,
    /// <summary>Sends the RPC to everyone. This client does not execute the RPC. New players get the RPC when they join as it's buffered (until this client leaves).</summary>
    OthersBuffered,
    /// <summary>Sends the RPC to everyone in the same team as the sender. This client does not execute the RPC.</summary>
    Team,
}

public static class RPCTargetsMethods
{
    public static bool IsInvokedInPlace(this RPCTargets target)
    {
        switch (target)
        {
            case RPCTargets.AllBuffered:
            case RPCTargets.All:
            case RPCTargets.Team:
                return true;
            case RPCTargets.Others:
            case RPCTargets.Specified:
            case RPCTargets.OthersBuffered:
                return false;
            case RPCTargets.Server:
                return MyComponents.NetworkManagement.isServer;
            default:
                throw new UnhandledSwitchCaseException(target);
        }
    }

    public static bool IsSentToNetwork(this RPCTargets target)
    {
        if (target == RPCTargets.Server && MyComponents.NetworkManagement.isServer)
            return false;
        return true;
    }

    public static MessageFlags GetFlags(this RPCTargets targets)
    {
        MessageFlags flags = MessageFlags.Reliable;
        switch (targets)
        {
            case RPCTargets.All:
            case RPCTargets.Others:
            case RPCTargets.AllBuffered:
            case RPCTargets.OthersBuffered:
            case RPCTargets.Team:
                break;
            case RPCTargets.Specified:
            case RPCTargets.Server:
                flags |= MessageFlags.NotDistributed;
                break;
            default:
                throw new UnhandledSwitchCaseException(targets);
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
            case RPCTargets.Team:
                break;
            default:
                throw new UnhandledSwitchCaseException(targets);
        }
        switch (targets)
        {
            case RPCTargets.AllBuffered:
            case RPCTargets.OthersBuffered:
                flags |= MessageFlags.SceneDependant;
                break;
            case RPCTargets.All:
            case RPCTargets.Server:
            case RPCTargets.Specified:
            case RPCTargets.Others:
            case RPCTargets.Team:
                break;
            default:
                throw new UnhandledSwitchCaseException(targets);
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
            case RPCTargets.Team:
                flags |= MessageFlags.SentToTeam;
                break;
            default:
                throw new UnhandledSwitchCaseException(targets);
        }
        return flags;
    }
}
