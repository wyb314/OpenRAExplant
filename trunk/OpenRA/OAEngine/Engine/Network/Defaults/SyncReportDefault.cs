using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Network.Interfaces;

namespace Engine.Network.Defaults
{
    public class SyncReportDefault : ISyncReport
    {
        public void DumpSyncReport(int frame, IEnumerable<ClientOrder> orders)
        {
            throw new NotImplementedException();
        }

        public void UpdateSyncReport()
        {
            throw new NotImplementedException();
        }
    }
}
