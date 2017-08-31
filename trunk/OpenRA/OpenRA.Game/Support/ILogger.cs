using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRA.Support
{
    public interface ILogger
    {

        void AddChannel(string channelName, string baseFilename);

        void Log(string message, string channelName = null, bool couldUseNativeDebug = false);

        void LogWarning(string message, string channelName = null, bool couldUseNativeDebug = false);

        void LogError(string message, string channelName = null, bool couldUseNativeDebug = false);

        void Assert(bool condition, string message, string channelName = null, bool couldUseNativeDebug = false);

        void LogException(Exception exception, string channelName = null, bool couldUseNativeDebug = false);
    }
}
