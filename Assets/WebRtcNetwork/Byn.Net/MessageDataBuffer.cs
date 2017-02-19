using System;

namespace Byn.Net
{
	public interface MessageDataBuffer : IDisposable
	{
		byte[] Buffer
		{
			get;
		}

		int Offset
		{
			get;
		}

		int ContentLength
		{
			get;
		}
	}
}
