using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRA.GameRules
{
    public class SoundInfo
    {
        public readonly Dictionary<string, string[]> Variants = new Dictionary<string, string[]>();
        public readonly Dictionary<string, string[]> Prefixes = new Dictionary<string, string[]>();
        public readonly Dictionary<string, string[]> Voices = new Dictionary<string, string[]>();
        public readonly Dictionary<string, string[]> Notifications = new Dictionary<string, string[]>();
        public readonly string DefaultVariant = ".aud";
        public readonly string DefaultPrefix = "";
        public readonly HashSet<string> DisableVariants = new HashSet<string>();
        public readonly HashSet<string> DisablePrefixes = new HashSet<string>();


        private Dictionary<string, SoundPool> voicePools;
        public Dictionary<string, SoundPool> VoicePools
        {
            get
            {
                if (voicePools == null)
                {
                    voicePools = Voices.ToDictionary(a => a.Key, a => new SoundPool(a.Value));
                }
                return voicePools;
            }

        }

        private Dictionary<string, SoundPool> notificationsPools;

        public Dictionary<string, SoundPool> NotificationsPools
        {
            get
            {
                if (notificationsPools == null)
                {
                    notificationsPools = Notifications.ToDictionary(a => a.Key, a => new SoundPool(a.Value));
                }
                return notificationsPools;
            }
        }

        public SoundInfo(MiniYaml y)
        {
            FieldLoader.Load(this, y);
            
        }
    }

    public class SoundPool
    {
        readonly string[] clips;
        readonly List<string> liveclips = new List<string>();

        public SoundPool(params string[] clips)
        {
            this.clips = clips;
        }

        public string GetNext()
        {
            if (liveclips.Count == 0)
                liveclips.AddRange(clips);

            if (liveclips.Count == 0)
                return null;        /* avoid crashing if there's no clips at all */

            var i = Game.CosmeticRandom.Next(liveclips.Count);
            var s = liveclips[i];
            liveclips.RemoveAt(i);
            return s;
        }
    }
}
