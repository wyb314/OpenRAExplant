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

namespace Engine.Support
{
	public static class Log
	{
	    private static ILogger innerLogger;

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
