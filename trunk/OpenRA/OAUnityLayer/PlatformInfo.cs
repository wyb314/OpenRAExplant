﻿using System;
using OpenRA;
using OpenRA.Support;
using UnityEngine;
using ILogger = OpenRA.Support.ILogger;

namespace OAUnityLayer
{
    public class PlatformInfo : IPlatformImpl
    {
        public const string FileFolderName = "File";

        public PlatformType currentPlatform { private set; get; }

        public System.Action<float> Tick { set; get; }

        public System.Action OnApplicationQuit { set; get; }

        public ILogger Logger { private set ; get; }

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


        public void GatherInfomation()
        {
            
#if UNITY_EDITOR
            currentPlatform = PlatformType.EDITOR;
#if SIMULATE_SANDBOX
            gameContentsDir = Application.dataPath + @"/../../SimulatePersistentDataPath/" + FileFolderName;
#else
            gameContentsDir = Application.persistentDataPath + @"/" + FileFolderName;
#endif
                  
#elif UNITY_STANDALONE_WIN
            currentPlatform = PlatformType.Windows;
            gameContentsDir = Application.dataPath + @"/SimulatePersistentDataPath/" + FileFolderName;
#elif UNITY_STANDALONE_OSX
            currentPlatform = PlatformType.OSX;
            gameContentsDir = Application.dataPath + @"/SimulatePersistentDataPath/" + FileFolderName;
#else

#if !UNITY_EDITOR && UNITY_IOS
            currentPlatform = PlatformType.IPhonePlayer;
            gameContentsDir = Application.persistentDataPath + @"/" + FileFolderName;
#elif !UNITY_EDITOR && UNITY_ANDROID
            currentPlatform = PlatformType.Android;
            gameContentsDir = Application.persistentDataPath + @"/" + FileFolderName;
#endif
#endif

        }

        public void SetLogger(ILogger logger)
        {
            this.Logger = logger;
        }


    }
}