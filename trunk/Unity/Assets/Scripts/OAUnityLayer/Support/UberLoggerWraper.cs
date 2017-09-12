using System;
using System.Collections.Generic;
using Engine.Support;
using UnityEngine;

namespace OAUnityLayer.Support
{
    public sealed class UberLoggerWraper : Engine.Support.ILogger
    {
        private OaUberLoggerFile uberLogger;
        
        public void Initialize(string logFolderPath = null)
        {
            uberLogger = new OaUberLoggerFile(true);
            uberLogger.SetLogPath(logFolderPath);
            UberLogger.Logger.AddLogger(uberLogger,false);
        }

        public void AddChannel(string channelName, string baseFilename)
        {
            if (uberLogger != null)
            {
                uberLogger.AddChannel(channelName,baseFilename);
            }
        }

        public void Assert(bool condition, string message, string channelName = null, bool couldUseNativeDebug = false)
        {
            UberDebug.LogChannel(channelName,message);
        }

        public void Log(string message, string channelName = null, bool couldUseNativeDebug = false)
        {
            UberDebug.LogChannel(channelName, message);

            if (couldUseNativeDebug)
            {
                Debug.Log(message);
            }
        }

        public void LogError(string message, string channelName = null, bool couldUseNativeDebug = false)
        {
            UberDebug.LogErrorChannel(channelName,message);

            if (couldUseNativeDebug)
            {
                Debug.LogError(message);
            }
        }

        public void LogException(Exception exception, string channelName = null, bool couldUseNativeDebug = false)
        {
            UberDebug.LogErrorChannel(channelName,exception.Message);

            if (couldUseNativeDebug)
            {
                Debug.LogException(exception);
            }
        }

        public void LogWarning(string message, string channelName = null, bool couldUseNativeDebug = false)
        {
            UberDebug.LogWarningChannel(channelName,message);

            if (couldUseNativeDebug)
            {
                Debug.LogWarning(message);
            }
        }
    }
}
