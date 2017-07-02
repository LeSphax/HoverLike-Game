using System;
using System.Diagnostics;

namespace Byn.Common
{
	public static class SLog
	{
		public static readonly string TAG_WARNING = "WARNING";

		public static readonly string TAG_ERROR = "ERROR";

		public static readonly string TAG_EXCEPTION = "EXCEPTION";

		public static readonly string TAG_INFO = "INFO";

		public static readonly string TAG_DEBUG = "DEBUG";

		public static readonly string TAG_VERBOSE = "VERBOSE";

		private static Action<object, string[]> sLogger = null;

		public static void SetLogger(Action<object, string[]> logger)
		{
			SLog.sLogger = logger;
		}

		public static void LogException(object msg, params string[] tags)
		{
			if (SLog.sLogger != null)
			{
				SLog.LogArray(msg, SLog.MergeTags(tags, new string[]
				{
					SLog.TAG_EXCEPTION
				}));
			}
		}

		public static void LE(object msg, params string[] tags)
		{
			if (SLog.sLogger != null)
			{
				SLog.LogArray(msg, SLog.MergeTags(tags, new string[]
				{
					SLog.TAG_ERROR
				}));
			}
		}

		public static void LW(object msg, params string[] tags)
		{
			if (SLog.sLogger != null)
			{
				SLog.LogArray(msg, SLog.MergeTags(tags, new string[]
				{
					SLog.TAG_WARNING
				}));
			}
		}

		[Conditional("DEBUG")]
		public static void LV(object msg, params string[] tags)
		{
			if (SLog.sLogger != null)
			{
				SLog.LogArray(msg, SLog.MergeTags(tags, new string[]
				{
					SLog.TAG_VERBOSE
				}));
			}
		}

		[Conditional("DEBUG")]
		public static void LD(object msg, params string[] tags)
		{
			if (SLog.sLogger != null)
			{
				SLog.LogArray(msg, SLog.MergeTags(tags, new string[]
				{
					SLog.TAG_DEBUG
				}));
			}
		}

		public static void L(object msg, params string[] tags)
		{
			if (SLog.sLogger != null)
			{
				SLog.LogArray(msg, SLog.MergeTags(tags, new string[]
				{
					SLog.TAG_INFO
				}));
			}
		}

		private static string[] MergeTags(string[] tags, params string[] newTags)
		{
			int startPos = newTags.Length;
			Array.Resize<string>(ref newTags, newTags.Length + tags.Length);
			Array.Copy(tags, 0, newTags, startPos, tags.Length);
			return newTags;
		}

		private static void LogArray(object obj, string[] tags)
		{
			if (SLog.sLogger != null)
			{
				SLog.sLogger(obj, tags);
			}
		}
	}
}
