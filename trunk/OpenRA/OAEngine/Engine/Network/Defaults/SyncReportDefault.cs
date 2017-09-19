using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Network.Interfaces;
using Engine.Support;

namespace Engine.Network.Defaults
{
    public class SyncReportDefault : ISyncReport
    {
        public void DumpSyncReport(int frame, IEnumerable<ClientOrder> orders)
        {
            Log.Write("wyb", "DumpSyncReport is invoke!");
        }

        public void UpdateSyncReport()
        {
            //Log.Write("wyb", "UpdateSyncReport is invoke!");
        }
    }
}
