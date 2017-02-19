using System;
using System.Linq;

namespace Byn.Net
{
	public static class DefaultValues
	{
		private static string[] mDefaultIceServer = new string[]
		{
			"stun:stun.l.google.com:19302"
		};

		private static string mDefaultSignalingServer = "wss://because-why-not.com:12777";

		public static string[] DefaultIceServers
		{
			get
			{
				return DefaultValues.mDefaultIceServer.ToArray<string>();
			}
		}

		public static string DefaultSignalingServer
		{
			get
			{
				return DefaultValues.mDefaultSignalingServer;
			}
		}
	}
}
