using System.Collections;
using System.Collections.Generic;
using OAUnityLayer;
using UnityEngine;
using OpenRA.Support;
using System.IO;
using OAUnityLayer.Support;
using OpenRA;
using OpenRA.Network;

public class Test0 : MonoBehaviour
{
    private PlatformInfo platformInfo;
    // Use this for initialization
    void Start()
    {
        Time.fixedDeltaTime = ((float)Game.Timestep)/1000;
    }

    // Update is called once per frame
    void Update()
    {
        if (platformInfo != null)
        {
            platformInfo.RenderTick(Time.fixedDeltaTime);
        }
    }

    void FixedUpdate()
    {
        if (platformInfo != null)
        {
            platformInfo.LogicTick(Time.fixedDeltaTime);
        }
    }

    //Game.Mod=ra
    void OnGUI()
    {
        if (GUILayout.Button("Create Folder"))
        {
            platformInfo = new PlatformInfo();
            platformInfo.GatherInfomation();
            UberLoggerWraper logger = new UberLoggerWraper();
            logger.Initialize(platformInfo.GameContentsDir);
            platformInfo.SetLogger(logger);
            

            Program.Run(new string[] { "Game.Mod=ra" }, platformInfo);


        }


        if (GUILayout.Button("Create Local Server"))
        {
            Game.CreateAndStartLocalServer("wyb", null);
        }

        if (GUILayout.Button("Start Game"))
        {
            Game.StartGame("wyb", WorldType.Regular);
        }

        if (GUILayout.Button("Send a order"))
        {
            Order order = Order.testWyb();
            Game.OrderManager.IssueOrder(order);
        }
    }

    void OnDestroy()
    {
        if (platformInfo != null)
        {
            platformInfo.OnApplicationQuit();
        }
    }



}
