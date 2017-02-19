using Byn.Common;
using System;
using System.Collections.Generic;

namespace Byn.Net
{
	public class ByteArrayBuffer : MessageDataBuffer, IDisposable
	{
		public static bool LOG_GC_CALLS;

		public byte[] array;

		private int positionWrite;

		private int positionRead;

		private int offset;

		private bool mFromPool = true;

		private bool mDisposed;

		private static List<ByteArrayBuffer>[] sPool;

		private static int[] MultiplyDeBruijnBitPosition;

		public int PositionWriteRelative
		{
			get
			{
				return this.positionWrite;
			}
			set
			{
				this.positionWrite = value;
			}
		}

		public int PositionWriteAbsolute
		{
			get
			{
				return this.positionWrite + this.offset;
			}
			set
			{
				this.positionWrite = value - this.offset;
			}
		}

		public int PositionReadRelative
		{
			get
			{
				return this.positionRead;
			}
		}

		public int PositionReadAbsolute
		{
			get
			{
				return this.positionRead + this.offset;
			}
		}

		public int Offset
		{
			get
			{
				return this.offset;
			}
		}

		public byte[] Buffer
		{
			get
			{
				if (this.mDisposed)
				{
					throw new InvalidOperationException("Object is already disposed. No further use allowed.");
				}
				return this.array;
			}
		}

		public int ContentLength
		{
			get
			{
				if (this.mDisposed)
				{
					throw new InvalidOperationException("Object is already disposed. No further use allowed.");
				}
				return this.positionWrite;
			}
			set
			{
				this.positionWrite = value;
			}
		}

		public bool IsDisposed
		{
			get
			{
				return this.mDisposed;
			}
		}

		private ByteArrayBuffer(int size)
		{
			this.mFromPool = true;
			this.array = new byte[size];
			this.offset = 0;
			this.positionWrite = 0;
			this.positionRead = 0;
		}

		public ByteArrayBuffer(byte[] arr) : this(arr, 0, arr.Length)
		{
		}

		public ByteArrayBuffer(byte[] arr, int offset, int length)
		{
			this.mFromPool = false;
			this.array = arr;
			this.offset = offset;
			this.positionRead = 0;
			this.positionWrite = length;
		}

		private void Reset()
		{
			this.mDisposed = false;
			this.positionRead = 0;
			this.positionWrite = 0;
		}

		~ByteArrayBuffer()
		{
			if (!this.mDisposed && this.mFromPool && ByteArrayBuffer.LOG_GC_CALLS)
			{
				SLog.LW("ByteArrayBuffer wasn't disposed.", new string[0]);
			}
		}

		public void CopyFrom(byte[] arr, int srcOffset, int len)
		{
			System.Buffer.BlockCopy(arr, srcOffset, this.array, this.offset, len);
			this.positionWrite = len;
		}

		static ByteArrayBuffer()
		{
			ByteArrayBuffer.LOG_GC_CALLS = false;
			ByteArrayBuffer.sPool = new List<ByteArrayBuffer>[32];
			ByteArrayBuffer.MultiplyDeBruijnBitPosition = new int[]
			{
				0,
				1,
				28,
				2,
				29,
				14,
				24,
				3,
				30,
				22,
				20,
				15,
				25,
				17,
				4,
				8,
				31,
				27,
				13,
				23,
				21,
				19,
				16,
				7,
				26,
				12,
				18,
				6,
				11,
				5,
				10,
				9
			};
			for (int i = 0; i < ByteArrayBuffer.sPool.Length; i++)
			{
				ByteArrayBuffer.sPool[i] = new List<ByteArrayBuffer>();
			}
		}

		private static int GetPower(uint anyPowerOfTwo)
		{
			uint index = anyPowerOfTwo * 125613361u >> 27;
			return ByteArrayBuffer.MultiplyDeBruijnBitPosition[(int)index];
		}

		private static uint NextPowerOfTwo(uint v)
		{
			v |= v >> 1;
			v |= v >> 2;
			v |= v >> 4;
			v |= v >> 8;
			v |= v >> 16;
			v += 1u;
			return v;
		}

		public static ByteArrayBuffer Get(int size, bool enforceZeroOffset = false)
		{
			uint pw = ByteArrayBuffer.NextPowerOfTwo((uint)size);
			if (pw < 128u)
			{
				pw = 128u;
			}
			int index = ByteArrayBuffer.GetPower(pw);
			if (ByteArrayBuffer.sPool[index].Count == 0)
			{
				return new ByteArrayBuffer((int)pw);
			}
			List<ByteArrayBuffer> expr_38 = ByteArrayBuffer.sPool[index];
			ByteArrayBuffer buff = expr_38[expr_38.Count - 1];
			expr_38.RemoveAt(expr_38.Count - 1);
			buff.Reset();
			return buff;
		}

		public void Dispose()
		{
			if (this.mDisposed)
			{
				throw new InvalidOperationException("Object is already disposed. No further use allowed.");
			}
			this.mDisposed = true;
			if (this.mFromPool)
			{
				int index = ByteArrayBuffer.GetPower((uint)this.array.Length);
				ByteArrayBuffer.sPool[index].Add(this);
			}
		}
	}
}
