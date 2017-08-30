using System.Linq;
using OpenRA.Traits;

namespace OpenRA
{
    public static class VoiceExts
    {
        public static void PlayVoice(this Actor self, string phrase)
        {
            if (phrase == null)
                return;

            foreach (var voiced in self.TraitsImplementing<IVoiced>())
            {
                if (string.IsNullOrEmpty(voiced.VoiceSet))
                    return;

                voiced.PlayVoice(self, phrase, self.Owner.Faction.InternalName);
            }
        }

        public static bool HasVoice(this Actor self, string voice)
        {
            return self.TraitsImplementing<IVoiced>().Any(x => x.HasVoice(self, voice));
        }

    }
}
