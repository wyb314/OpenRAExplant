
using System.IO;
using OpenRA.FileFormats;
using OpenRA.FileSystem;

namespace OpenRA.GameRules
{
    public class MusicInfo
    {
        public readonly string Filename;
        public readonly string Title;
        public readonly bool Hidden;

        public int Length { get; private set; } // seconds
        public bool Exists { get; private set; }

        public MusicInfo(string key, MiniYaml value)
        {
            Title = value.Value;

            var nd = value.ToDictionary();
            if (nd.ContainsKey("Hidden"))
                bool.TryParse(nd["Hidden"].Value, out Hidden);

            var ext = nd.ContainsKey("Extension") ? nd["Extension"].Value : "aud";
            Filename = (nd.ContainsKey("Filename") ? nd["Filename"].Value : key) + "." + ext;
        }

        //public void Load(IReadOnlyFileSystem fileSystem)
        //{
        //    Stream stream;
        //    if (!fileSystem.TryOpen(Filename, out stream))
        //        return;

        //    try
        //    {
        //        //Exists = true;
        //        //foreach (var loader in Game.ModData.SoundLoaders)
        //        //{
        //        //    ISoundFormat soundFormat;
        //        //    if (loader.TryParseSound(stream, out soundFormat))
        //        //    {
        //        //        Length = (int)soundFormat.LengthInSeconds;
        //        //        soundFormat.Dispose();
        //        //        break;
        //        //    }
        //        //}
        //    }
        //    finally
        //    {
        //        stream.Dispose();
        //    }
        //}
    }
}
