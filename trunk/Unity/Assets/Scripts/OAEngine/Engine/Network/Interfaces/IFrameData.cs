using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Network.Interfaces
{
    public interface IFrameData
    {
        Dictionary<int, int> clientQuitTimes { get; }

        Dictionary<int, Dictionary<int, byte[]>> framePackets { get; }

        /// <summary>
        /// 客户端退出游戏
        /// </summary>
        /// <param name="clientId">客户端id</param>
        /// <param name="lastClientFrame">客户端退出的帧索引</param>
        void ClientQuit(int clientId, int lastClientFrame);


        void AddFrameOrders(int clientId, int frame, byte[] orders);

        /// <summary>
        /// 值为frame的这一帧当前是否准备好
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        bool IsReadyForFrame(int frame);

        IEnumerable<ClientOrder> OrdersForFrame(IOrderManager orderManager, INetWorld world, int frame);
        

    }
}
