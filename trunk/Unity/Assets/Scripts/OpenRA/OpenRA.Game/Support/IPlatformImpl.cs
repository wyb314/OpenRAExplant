using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenRA.Support
{
    public delegate void LogicTickFun(float elapsedTime);

    public delegate void RenderTickFun(float elapsedTime);


    public interface IPlatformImpl
    {
        PlatformType currentPlatform { get; }

        /// <summary>
        /// 获取游戏资源目录
        /// </summary>
        string GameContentsDir { get; }

        /// <summary>
        /// 游戏逻辑循环
        /// </summary>
        //void LogicTick(float elapsedTime);

        /// <summary>
        /// 游戏渲染循环
        /// </summary>
        //void RenderTick(float elapsedTime);

        /// <summary>
        /// 注册游戏逻辑Tick
        /// </summary>
        /// <param name="logicTick"></param>
        void RegisterLogicTick(LogicTickFun logicTick);

        /// <summary>
        /// 注册游戏渲染Tick
        /// </summary>
        /// <param name="logicTick"></param>
        void RegisterRenderTick(RenderTickFun logicTick);

        /// <summary>
        /// 游戏退出回调
        /// </summary>
        Action OnApplicationQuit { set; get; }

        /// <summary>
        /// 设备日志输出器
        /// </summary>
        ILogger Logger { get; }
    }
}
