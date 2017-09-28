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
using Engine.Primitives;
using Engine.Support;
using OAUnityLayer.Factories;
using OAUnityLayer.Support;
using Game = Engine.Game;

public class Test0 : MonoBehaviour
{
    private PlatformInfo platformInfo;
    // Use this for initialization

    //public Transform playTran;

    public GUIStyle guiStyle = new GUIStyle();

    private GameInputerGetter inputer;
    void Start()
    {
        guiStyle.fontSize = 20;
        guiStyle.normal.textColor = UnityEngine.Color.green;
        guiStyle.alignment = TextAnchor.UpperCenter;

        Application.targetFrameRate = 40;
        inputer = new GameInputerGetter();
        Time.fixedDeltaTime = ((float)Engine.Game.Timestep)/1000;

    }

    // Update is called once per frame
    void Update()
    {
        if (this.pause)
        {
            return;
        }

        if (platformInfo != null)
        {
            platformInfo.inputter.Update();
        }
        
        //this.GetJoystickInput();

    }

    private int curFacing;
    private int targetFaceing;

    private void GetJoystickInput()
    {
        float h = this.inputer.GetAxis("Horizontal");
        float v = this.inputer.GetAxis("Vertical");

        float sqrLength = h * h + v * v;
        if (sqrLength > 0)
        {
            float rad = Mathf.Atan2(v, h);
            this.targetFaceing = (int)(rad * 128 / Mathf.PI);
        }
    }

    void FixedUpdate()
    {
        if (this.pause)
        {
            return;
        }
        if (platformInfo != null)
        {
            platformInfo.LogicTick(Time.fixedDeltaTime);
        }

        //if (this.curFacing != this.targetFaceing)
        //{
        //    float rad = this.targetFaceing*Mathf.PI/128;

        //    this.playTran.eulerAngles = new Vector3(0,rad * Mathf.Rad2Deg,0);
        //}
    }

    public bool pause = false;
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
            platformInfo.inputGetter = inputer;
            platformInfo.actorRendererFactory = new NormalActorRendererFactory();

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

        if (GUILayout.Button("Pause"+!pause))
        {
            pause = !pause;
        }

        if (Engine.Game.OrderManager != null)
        {
            int clientId = Engine.Game.OrderManager.Connection.LocalClientId;
            if (Engine.Game.OrderManager.LobbyInfo!= null)
            {
                ClientDefault client =
               Engine.Game.OrderManager.LobbyInfo.ClientWithIndex(clientId);
                string info = string.Format("TimeStep->{0} Id->{1} Admin->{2} NetFrame->{3} LocalFrame->{4}", Game.Timestep, clientId, client.IsAdmin,
                    Engine.Game.OrderManager.NetFrameNumber, Engine.Game.OrderManager.LocalFrameNumber);
                GUILayout.Label(info, this.guiStyle);
            }
           
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
