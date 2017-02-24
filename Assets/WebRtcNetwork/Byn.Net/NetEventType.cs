namespace Byn.Net
{
    public enum NetEventType : byte
	{
		Invalid,
		UnreliableMessageReceived,
		ReliableMessageReceived,
		ServerInitialized,
		ServerConnectionFailed,
		ServerClosed,
		NewConnection,
		ConnectionFailed,
		Disconnected,
		FatalError = 100,
		Warning,
		Log,
        UserCommand,
	}
}
