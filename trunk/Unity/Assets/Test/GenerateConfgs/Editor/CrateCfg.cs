using System.Collections;
using System.Collections.Generic;
using Engine.Maps;
using UnityEngine;
using UnityEditor;

public class CrateCfg
{
    [MenuItem("Configs/Create Map Config")]
    public static void CrateMapConfig()
    {
        Map map1 = new Map();

        List<PlayerReference> players = new List<PlayerReference>();
        map1.Players = players;

        //players.Add();

        List<ActorReference> actors = new List<ActorReference>();
        map1.Actors = actors;



    }


}
