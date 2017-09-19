using System.Collections;
using System.Collections.Generic;
using OAUnityLayer;
using UnityEngine;
using System.IO;
using System.Net;
using Engine;
using Engine.Network;
using Engine.Network.Defaults;
using Engine.Network.Enums;
using Engine.Support;
using OAUnityLayer.Support;
using Game = Engine.Game;

public class Test0 : MonoBehaviour
{
    private PlatformInfo platformInfo;
    // Use this for initialization
    void Start()
    {
        Time.fixedDeltaTime = ((float)Engine.Game.Timestep)/1000;
    }

    // Update is called once per frame
    void Update()
    {
        //if (platformInfo != null)
        //{
        //    platformInfo.Tick(Time.fixedDeltaTime);
        //}
    }

    void FixedUpdate()
    {
        if (platformInfo != null)
        {
            platformInfo.Tick(Time.fixedDeltaTime);
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


        //if (GUILayout.Button("Create and join Local Server"))
        //{
        //    var orders = new[] 
        //    {
        //        Order.Command("state {0}".F(ClientState.Ready))
        //    };
        //    Engine.Game.CreateAndStartLocalServer("wyb", orders);
        //}

        if(GUILayout.Button("Create and join remote Server"))
        {
            //var orders = new[]
            //{
            //    Order.Command("state {0}".F(ClientState.Ready))
            //};
            Engine.Game.JoinServer(IPAddress.Loopback.ToString(), 1234, "");
        }

        if (GUILayout.Button("Send ready"))
        {
            var orders = new[]
            {
                Order.Command("state {0}".F(ClientState.NotReady)),
            };
            Engine.Game.OrderManager.IssueOrders(orders);

        }

        if (GUILayout.Button("Start Game"))
        {
            var orders = new[]
            {
                Order.Command("startgame"),
            };
            Engine.Game.OrderManager.IssueOrders(orders);
            
        }
        //if (GUILayout.Button("Start Mission game"))
        //{
        //    Order order = Order.Command("state {0}".F(ClientState.Ready));
        //    Engine.Game.OrderManager.IssueOrder(order);
        //}

        //if (GUILayout.Button("Send a order"))
        //{
        //    Order order = Order.Command("hello world!");
        //    Engine.Game.OrderManager.IssueOrder(order);
        //}
    }

    void OnDestroy()
    {
        if (platformInfo != null)
        {
            platformInfo.OnApplicationQuit();
        }
    }



}
