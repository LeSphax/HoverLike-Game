namespace Byn.Net
{
    public enum NetEventType : byte
    {
        Invalid,
        UnreliableMessageReceived,
        ReliableMessageReceived,
        RoomCreated,
        RoomCreationFailed,
        RoomJoinFailed = 5,
        RoomClosed,
        NewConnection,
        ConnectionFailed,
        Disconnected,
        ConnectionToSignalingServerEstablished = 10,
        Allowed_To_Enter,
        FatalError = 100,
        Warning,
        Log,
        UserCommand,
        SignalingConnectionFailed = 200,
    }
}
