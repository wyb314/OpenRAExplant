using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Engine;
using Server;
using YamlConfigSimpleMake.ConfigsMake;

namespace YamlConfigSimpleMake
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerPlatformInfo platformInfo = new ServerPlatformInfo();
            platformInfo.GatherInfomation();
            Platform.SetCurrentPlatform(platformInfo);
            MapConfigMake.Make();
            ManifestConfigMake.Make();
            SettingConfigMake.Make();
        }
    }
}
