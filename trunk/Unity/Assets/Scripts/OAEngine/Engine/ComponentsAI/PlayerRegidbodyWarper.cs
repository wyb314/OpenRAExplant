using System;
using System.Collections.Generic;
using Engine.Interfaces;
using Engine.Physics;
using TrueSyncPhysics;

namespace Engine.ComponentsAI
{
    public delegate void OnCollisionDetectedEvt(TSCollision2D collision2D);

    public class PlayerRegidbodyWarper : IRegidbodyWrapObject
    {

        private PlayerAgent agent;

        private IRender rendererProxy;

        public PlayerRegidbodyWarper(PlayerAgent agent)
        {
            this.agent = agent;
        }


        private int _layer;
        public int layer
        {
            get { return this._layer; }

            set { this._layer = value; }
        }

        public object GetComponentByType(Type type)
        {
            return this.agent.GetComponent(type);
        }

        public void OnDestroy()
        {
            if (this.rendererProxy != null)
            {
                this.rendererProxy.OnDestroy();
            }
        }


        private Dictionary<string, OnCollisionDetectedEvt> _evtsDir;

        public void RegisterCollisionEvents(string evtName , OnCollisionDetectedEvt evt)
        {
            if (_evtsDir == null)
            {
                _evtsDir = new Dictionary<string, OnCollisionDetectedEvt>();
            }

            if (!_evtsDir.ContainsKey(evtName))
            {
                _evtsDir.Add(evtName,evt);
            }
            else
            {
                throw new Exception("Can't not add same evt 2 or more times that event name is "+evtName);
            }
        }


        public void SendCollisionMessage(string callbackName, object arg)
        {
            if (this._evtsDir != null)
            {
                OnCollisionDetectedEvt evt = null;
                if (this._evtsDir.TryGetValue(callbackName,out evt))
                {
                    evt(arg as TSCollision2D);
                }
            }
        }

        public void SetActive(bool active)
        {
            if (this.rendererProxy != null)
            {
                this.rendererProxy.SetActive(active);
            }
        }
    }
}
