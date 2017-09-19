using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Network.Interfaces;

namespace Engine
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SyncAttribute : Attribute { }

    public interface ISync { }


    public static class Sync
    {

        public static void CheckSyncUnchanged(INetWorld world, Action fn)
        {
            CheckSyncUnchanged(world, () => { fn(); return true; });
        }

        static bool inUnsyncedCode = false;

        public static T CheckSyncUnchanged<T>(INetWorld world, Func<T> fn)
        {
            if (world == null)
                return fn();
            return fn();
            //var shouldCheckSync = Game.Settings.Debug.SanityCheckUnsyncedCode;
            //var sync = shouldCheckSync ? world.SyncHash() : 0;
            //var prevInUnsyncedCode = inUnsyncedCode;
            //inUnsyncedCode = true;

            //try
            //{
            //    return fn();
            //}
            //finally
            //{
            //    inUnsyncedCode = prevInUnsyncedCode;
            //    if (shouldCheckSync && sync != world.SyncHash())
            //        throw new InvalidOperationException("CheckSyncUnchanged: sync-changing code may not run here");
            //}
        }

        public static void AssertUnsynced(string message)
        {
            //if (!inUnsyncedCode)
            //    throw new InvalidOperationException(message);
        }
    }
}
