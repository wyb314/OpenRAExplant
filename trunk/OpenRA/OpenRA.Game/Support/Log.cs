#region Copyright & License Information
/*
 * Copyright 2007-2017 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using OpenRA.Support;

namespace OpenRA
{
	public struct ChannelInfo
	{
		public string Filename;
		public TextWriter Writer;
	}

	public static class Log
	{
	    private static ILogger innerLogger;

		//static readonly Dictionary<string, ChannelInfo> Channels = new Dictionary<string, ChannelInfo>();

		//static IEnumerable<string> FilenamesForChannel(string channelName, string baseFilename)
		//{
		//	var path = Platform.SupportDir + "Logs";
		//	Directory.CreateDirectory(path);

		//	for (var i = 0;; i++)
		//		yield return Path.Combine(path, i > 0 ? "{0}.{1}".F(baseFilename, i) : baseFilename);
		//}

		//public static ChannelInfo Channel(string channelName)
		//{
		//	ChannelInfo info;
		//	lock (Channels)
		//		if (!Channels.TryGetValue(channelName, out info))
		//			throw new ArgumentException("Tried logging to non-existent channel " + channelName, "channelName");

		//	return info;
		//}


	    public static void SetLogger(ILogger logger)
	    {
	        innerLogger = logger;

	    }

		public static void AddChannel(string channelName, string baseFilename)
		{
		    if (innerLogger != null)
		    {
                innerLogger.AddChannel(channelName,baseFilename);
		    }
        }

		public static void Write(string channel, string value, bool couldUseNativeDebug = false)
		{
            if (innerLogger != null)
            {
                innerLogger.Log(value,channel, couldUseNativeDebug);
            }
		}

		public static void Write(string channel, string format, params object[] args)
		{
            if (innerLogger != null)
            {
                innerLogger.Log(string.Format(format, args), channel);
            }
		}

	    public static void LogWarning(string message, string channelName = null, bool couldUseNativeDebug = false)
	    {
            if (innerLogger != null)
            {
                innerLogger.LogWarning(message,channelName,couldUseNativeDebug);
            }
        }

	    public static void LogError(string message, string channelName = null, bool couldUseNativeDebug = false)
	    {
            if (innerLogger != null)
            {
                innerLogger.LogError(message, channelName, couldUseNativeDebug);
            }
        }

	    public static void Assert(bool condition, string message, string channelName = null,
	        bool couldUseNativeDebug = false)
	    {
            if (innerLogger != null)
            {
                innerLogger.Assert(condition, message, channelName);
            }
        }

	    public static void LogException(Exception exception, string channelName = null, bool couldUseNativeDebug = false)
	    {
            if (innerLogger != null)
            {
                innerLogger.LogException(exception, channelName, couldUseNativeDebug);
            }
        }
    }
}
