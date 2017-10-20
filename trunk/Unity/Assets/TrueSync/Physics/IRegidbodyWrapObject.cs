using System;
using System.Collections.Generic;

namespace TrueSyncPhysics
{
    public interface IRegidbodyWrapObject
    {
        int layer { set;get; }

        void SetActive(bool active);

        System.Object GetComponentByType(Type type);

        void OnDestroy();

        void SendCollisionMessage(string callbackName,System.Object arg);
    }
}
