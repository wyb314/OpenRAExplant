using System;
using System.Collections.Generic;
using UberLogger;
using System.IO;
using OpenRA;
using UnityEngine;

namespace OAUnityLayer.Support
{
    public class OaUberLoggerFile : UberLogger.ILogger
    {
        public class ChannelInfo
        {
            public string channelName;
            public StreamWriter Writer;

            public void Log(LogInfo logInfo)
            {
                this.Writer.WriteLine(logInfo.Message);
            }

            public void Log(string logInfo)
            {
                this.Writer.WriteLine(logInfo);
            }

            public void OnDestroy()
            {
                this.Writer.Flush();
                this.Writer.Close();
            }
        }

        private Dictionary<string, ChannelInfo> chennelInfos = new Dictionary<string, ChannelInfo>();

        //private StreamWriter LogFileWriter;
        private bool IncludeCallStacks;

        public string logFolderPath { private set; get; }
        /// <summary>
        /// Constructor. Make sure to add it to UberLogger via Logger.AddLogger();
        /// filename is relative to Application.persistentDataPath
        /// if includeCallStacks is true it will dump out the full callstack for all logs, at the expense of big log files.
        /// </summary>
        public OaUberLoggerFile(bool includeCallStacks = true)
        {
            IncludeCallStacks = includeCallStacks;
        }


        public void SetLogPath(string folderPath = null)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                folderPath = Platform.platformInfo.GameContentsDir;
            }
            
            
            this.logFolderPath = folderPath+@"/Logs";
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            //var logPath = System.IO.Path.Combine(Application.persistentDataPath, filename);
            Debug.Log("Initialising file logging to " + logFolderPath);
        }


        public void AddChannel(string channelName, string baseFilename)
        {
            ChannelInfo info = GetChannelInfo(channelName);
            if (info == null)
            {
                info = this.CreateChannelInfo(baseFilename);

                this.chennelInfos.Add(channelName, info);
            }
        }

        public ChannelInfo CreateChannelInfo(string channel)
        {
            ChannelInfo info = new ChannelInfo();
            info.channelName = channel;
            string fileName = System.IO.Path.Combine(logFolderPath, channel);
            info.Writer = new StreamWriter(fileName, false);
            info.Writer.AutoFlush = true;

            return info;
        }

        public ChannelInfo GetChannelInfo(string channel)
        {
            ChannelInfo info = null;

            this.chennelInfos.TryGetValue(channel, out info);

            return info;
        }

        public void Log(LogInfo logInfo)
        {
            lock (this)
            {
                //LogFileWriter.WriteLine(logInfo.Message);
                ChannelInfo info = GetChannelInfo(logInfo.Channel);

                if (info == null)
                {
                    return;
                }
                info.Log(logInfo);

                if (IncludeCallStacks && logInfo.Callstack.Count > 0)
                {
                    foreach (var frame in logInfo.Callstack)
                    {
                        info.Log(frame.GetFormattedMethodName());
                    }
                    info.Log("");
                }
            }
        }


        public void OnDestroy()
        {
            foreach (var kvp in chennelInfos)
            {
                kvp.Value.OnDestroy();
            }
            this.chennelInfos.Clear();
        }

    }
}
