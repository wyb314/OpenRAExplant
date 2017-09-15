using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Network.Interfaces;
using Engine.Network.Server;
using Engine.Support;

namespace Engine.Network.Defaults.ServerTraits
{
    public class LobbySettingsNotification : ServerTrait, IClientJoined<ClientDefault>
    {
        public LobbySettingsNotification()
        {
        }
        public void ClientJoined(IServer<ClientDefault> server, IServerConnectoin<ClientDefault> conn)
        {
            if (server.LobbyInfo.ClientWithIndex(conn.PlayerIndex).IsAdmin)
                return;

            //    var defaults = new Session.Global();
            //    LobbyCommands.LoadMapSettings(server, defaults, server.Map.Rules);

            //    var options = server.Map.Rules.Actors["player"].TraitInfos<ILobbyOptions>()
            //        .Concat(server.Map.Rules.Actors["world"].TraitInfos<ILobbyOptions>())
            //        .SelectMany(t => t.LobbyOptions(server.Map.Rules));

            //    var optionNames = new Dictionary<string, string>();
            //    foreach (var o in options)
            //        optionNames[o.Id] = o.Name;

            //    foreach (var kv in server.LobbyInfo.GlobalSettings.LobbyOptions)
            //    {
            //        Session.LobbyOptionState def;
            //        string optionName;
            //        if (!defaults.LobbyOptions.TryGetValue(kv.Key, out def) || kv.Value.Value != def.Value)
            //            if (optionNames.TryGetValue(kv.Key, out optionName))
            //                server.SendOrderTo(conn, "Message", optionName + ": " + kv.Value.Value);
            //    }
        }
    }
}
