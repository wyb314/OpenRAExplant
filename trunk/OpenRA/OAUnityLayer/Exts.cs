using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Primitives;
using UnityEngine;

namespace OAUnityLayer
{
    public static class Exts
    {
       
        
        public static Vector3 ConvertWPos2UnityPos(this WPos pos)
        {
            return new Vector3(((float)pos.X)/1024,0,((float)pos.Y)/1024);
        }
    }
}
