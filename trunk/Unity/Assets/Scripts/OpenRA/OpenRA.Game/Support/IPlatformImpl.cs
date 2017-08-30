using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRA.Support
{
    public interface IPlatformImpl
    {
        PlatformType currentPlatform { get; }

        /// <summary>
        /// 获取游戏资源目录
        /// </summary>
        string GameContentsDir { get; }

    }
}
