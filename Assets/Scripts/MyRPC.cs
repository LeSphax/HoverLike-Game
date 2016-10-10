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
    /// <summary>Sends the RPC to MasterClient only. Careful: The MasterClient might disconnect before it executes the RPC and that might cause dropped RPCs.</summary>
    Server,
    /// <summary>Sends the RPC to everyone else and executes it immediately on this client. New players get the RPC when they join as it's buffered (until this client leaves).</summary>
    AllBuffered,
    /// <summary>Sends the RPC to everyone. This client does not execute the RPC. New players get the RPC when they join as it's buffered (until this client leaves).</summary>
    OthersBuffered,
    /// <summary>Sends the RPC to everyone (including this client) through the server.</summary>
    /// <remarks>
    /// This client executes the RPC like any other when it received it from the server.
    /// Benefit: The server's order of sending the RPCs is the same on all clients.
    /// </remarks>
    AllViaServer,
    /// <summary>Sends the RPC to everyone (including this client) through the server and buffers it for players joining later.</summary>
    /// <remarks>
    /// This client executes the RPC like any other when it received it from the server.
    /// Benefit: The server's order of sending the RPCs is the same on all clients.
    /// </remarks>
    AllBufferedViaServer
}

public static class RPCTargetsMethods
{
    public static bool IsInvokedInPlace(this RPCTargets target)
    {
        switch (target)
        {
            case RPCTargets.AllBuffered:
            case RPCTargets.All:
                return true;
            case RPCTargets.Others:
            case RPCTargets.OthersBuffered:
                return false;
            case RPCTargets.AllViaServer:
            case RPCTargets.Server:
            case RPCTargets.AllBufferedViaServer:
                return MyGameObjects.NetworkManagement.isServer;
            default:
                throw new UnhandledSwitchCaseException(target);
        }
    }
}
