using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Engine.Support;

namespace Engine
{
    public enum PlatformType
    {
        Unknown,
        EDITOR,
        Windows,
        OSX,
        IPhonePlayer,
        Android,
    }

    public static class Platform
    {
        public const string SeparatorChar = @"/";

        public static IPlatformImpl platformInfo;

        private static PlatformType currentPlatform = PlatformType.Unknown;

        public static PlatformType CurrentPlatform
        {
            get
            {
                return currentPlatform;
            }
        }

        private static string gameDir = string.Empty;

        public static string GameDir
        {
            get
            {
                return gameDir;
            }
        }

        private static string modsDir = string.Empty;

        public static string ModsDir
        {
            get { return modsDir; }
        }

        public static bool SetCurrentPlatform(IPlatformImpl info)
        {
            platformInfo = info;

            if (platformInfo != null)
            {
                currentPlatform = platformInfo.currentPlatform;

                gameDir = platformInfo.GameContentsDir + SeparatorChar + @"OARes";

                modsDir = Path.Combine(platformInfo.GameContentsDir, "mods");
            }
            return false;
        }

        private static string supportDir = string.Empty;

        public static string SupportDir
        {
            get
            {
                if (string.IsNullOrEmpty(supportDir))
                {
                    supportDir = GetSupportDir();
                }
                return supportDir;
            }
        }

        static string GetSupportDir()
        {
            // Use a local directory in the game root if it exists (shared with the system support dir)
            //var localSupportDir = Path.Combine(GameDir, "Support");
            //if (Directory.Exists(localSupportDir))
            //    return localSupportDir + Path.DirectorySeparatorChar;

            //var dir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //dir = AppDomain.CurrentDomain.BaseDirectory;

            //switch (CurrentPlatform)
            //{
            //    case PlatformType.Windows:
            //        dir += Path.DirectorySeparatorChar + "OpenRARes" + Path.DirectorySeparatorChar + "OpenRA";
            //        break;
            //    case PlatformType.OSX:
            //        dir += "/Library/Application Support/OpenRA";
            //        break;
            //    default:
            //        dir += "/.openra";
            //        break;
            //}

            var dir = GameDir + SeparatorChar + @"Contents";

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir + SeparatorChar;
        }




        public static string ResolvePath(string path)
        {
            path = path.TrimEnd(' ', '\t');

            // Paths starting with ^ are relative to the support dir
            if (path.StartsWith("^", StringComparison.Ordinal))
                path = SupportDir + path.Substring(1);

            // Paths starting with . are relative to the game dir
            if (path == ".")
                return GameDir + SeparatorChar;

            if (path.StartsWith("./", StringComparison.Ordinal) || path.StartsWith(".\\", StringComparison.Ordinal))
                path = GameDir + SeparatorChar + path.Substring(2);

            return path;
        }

        public static string ResolvePath(params string[] path)
        {
            return ResolvePath(path.Aggregate(Path.Combine));
        }

        //TODO::
        public static string SystemSupportDir { get { return ""; } }

    }
}
