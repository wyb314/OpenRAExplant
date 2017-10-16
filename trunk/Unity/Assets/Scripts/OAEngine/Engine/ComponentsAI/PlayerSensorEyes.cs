using System;
using System.Collections.Generic;
using Engine.ComponentsAI.AStarMachine;
using Engine.ComponentsAI.GOAP.Core;
using TrueSync;

namespace Engine.ComponentsAI
{
    public class PlayerSensorEyes
    {
        Agent Owner;
        //Transform Transform;

        public FP EyeRange = 6;
        public FP FieldOfView = 120;

        FP sqrEyeRange { get { return EyeRange * EyeRange; } }

        public PlayerSensorEyes(Agent agent)
        {
            this.Owner = agent;
        }

        // Update is called once per frame
        public void Tick()
        {
            //if (Owner.IsVisible == false)
            //    return;

            //if (Mission.Instance.CurrentGameZone == null)
            //{
            //    Owner.WorldState.SetWSProperty(E_PropKey.E_ALERTED, false);
            //    return;
            //}

            //List<Agent> enemies = this.Owner.world.Ememys;
            
            //if (Owner.WorldState.GetWSProperty(E_PropKey.E_ALERTED).GetBool() == true)
            //{
            //    if (enemies== null || enemies.Count == 0)
            //        Owner.WorldState.SetWSProperty(E_PropKey.E_ALERTED, false);
            //}
            //else
            //{
            //    if (enemies != null)
            //    {
            //        for (int i = 0; i < enemies.Count; i++)
            //        {
            //            if ((Owner.Position - enemies[i].Position).LengthSquared() < sqrEyeRange)
            //            {
            //                Owner.WorldState.SetWSProperty(E_PropKey.E_ALERTED, true);
            //                return;
            //            }
            //        }
            //    }
               

            //    Owner.WorldState.SetWSProperty(E_PropKey.E_ALERTED, false);
            //}

            List<Agent> enemies = this.Owner.world.GetEmemys(this.Owner);
            if (enemies == null || enemies.Count == 0)
            {
                if (Owner.WorldState.GetWSProperty(E_PropKey.E_ALERTED).GetBool() == true)
                {
                    Owner.WorldState.SetWSProperty(E_PropKey.E_ALERTED, false);
                }
            }
            else
            {
                if (enemies != null)
                {
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if ((Owner.Position - enemies[i].Position).LengthSquared() < sqrEyeRange)
                        {
                            Owner.WorldState.SetWSProperty(E_PropKey.E_ALERTED, true);
                            return;
                        }
                    }
                }


                Owner.WorldState.SetWSProperty(E_PropKey.E_ALERTED, false);
            }
        }

    }
}
