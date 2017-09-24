using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Support;
using System.IO;
using Engine.Network;

namespace Engine.Server.Logs
{
    public struct ChannelInfo
    {
        public string Filename;
        public TextWriter Writer;
    }
    public class ServerLogger : ILogger
    {
        readonly Dictionary<string, ChannelInfo> Channels = new Dictionary<string, ChannelInfo>();

        static IEnumerable<string> FilenamesForChannel(string channelName, string baseFilename)
        {
            var path = Platform.platformInfo.GameContentsDir+@"/Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            for (var i = 0; ; i++)
                yield return Path.Combine(path, i > 0 ? "{0}.{1}".F(baseFilename, i) : baseFilename);
        }

        public void AddChannel(string channelName, string baseFilename)
        {
            lock (Channels)
            {
                if (Channels.ContainsKey(channelName)) return;

                if (string.IsNullOrEmpty(baseFilename))
                {
                    Channels.Add(channelName, new ChannelInfo());
                    return;
                }

                foreach (var filename in FilenamesForChannel(channelName, baseFilename))
                    try
                    {
                        var writer = File.CreateText(filename);
                        writer.AutoFlush = true;

                        Channels.Add(channelName,
                            new ChannelInfo
                            {
                                Filename = filename,
                                Writer = TextWriter.Synchronized(writer)
                            });

                        return;
                    }
                    catch (IOException) { }
            }
        }

        public ChannelInfo Channel(string channelName)
        {
            ChannelInfo info;
            lock (Channels)
                if (!Channels.TryGetValue(channelName, out info))
                    throw new ArgumentException("Tried logging to non-existent channel " + channelName, "channelName");

            return info;
        }

        public void Assert(bool condition, string message, string channelName = null, bool couldUseNativeDebug = false)
        {
            var writer = Channel(channelName).Writer;
            if (writer == null)
                return;

            writer.WriteLine("Assert: "+message);
        }


        public void Log(string message, string channelName = null, bool couldUseNativeDebug = false)
        {
            var writer = Channel(channelName).Writer;
            if (writer == null)
                return;

            writer.WriteLine(message);
        }

        public void LogError(string message, string channelName = null, bool couldUseNativeDebug = false)
        {
            var writer = Channel(channelName).Writer;
            if (writer == null)
                return;

            writer.WriteLine("Error： "+message);
        }

        public void LogException(Exception exception, string channelName = null, bool couldUseNativeDebug = false)
        {
            var writer = Channel(channelName).Writer;
            if (writer == null)
                return;

            writer.WriteLine("Exception： " + exception.Message);
        }

        public void LogWarning(string message, string channelName = null, bool couldUseNativeDebug = false)
        {
            var writer = Channel(channelName).Writer;
            if (writer == null)
                return;

            writer.WriteLine("Warning: " + message);
        }
    }
}
