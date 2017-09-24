using System;
using Engine;
using Engine.Inputs;
using Engine.Interfaces;
using System.Collections.Generic;
using Engine.Support;
using ILogger = Engine.Support.ILogger;

namespace Server
{
    public class ServerPlatformInfo : IPlatformImpl
    {
        public const string FileFolderName = "File";

        public PlatformType currentPlatform { private set; get; }

        public System.Action<float> LogicTick { set; get; }

        //public Action<float> Tick { set; get; }

        public System.Action OnApplicationQuit { set; get; }

        public ILogger Logger { private set; get; }


        public IActorRendererFactory actorRendererFactory { set; get; }

        private string gameContentsDir;

        public string GameContentsDir
        {
            get
            {
                if (string.IsNullOrEmpty(gameContentsDir))
                {
                    throw new NullReferenceException("gameContentsDir is null or empty!");
                }
                return gameContentsDir;
            }
        }

        public IInputter inputter { private set; get; }

        public IInputsGetter inputGetter
        {
            get { return this.inputter.inputGetter; }

            set
            {
                this.inputter.inputGetter = value;
            }
        }

        public ServerPlatformInfo()
        {
            this.inputter = new GameInputter();
        }


        public void GatherInfomation()
        {
            currentPlatform = PlatformType.EDITOR;
            gameContentsDir = AppDomain.CurrentDomain.BaseDirectory + @"/GameDir/Files";


        }

        public void SetLogger(ILogger logger)
        {
            this.Logger = logger;
        }

    }
}
