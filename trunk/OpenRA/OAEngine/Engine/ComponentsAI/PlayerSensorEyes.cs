using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AStarMachine;

namespace Engine.ComponentsAI
{
    public class PlayerSensorEyes
    {
        Agent Owner;
        public float EyeRange = 6;
        public float FieldOfView = 120;

        float sqrEyeRange { get { return EyeRange * EyeRange; } }

        public void Tick()
        {

        }

    }
}
