using System;
using OpenRA.FileFormats;

namespace OpenRA.Mods.Common.LoadScreens
{
    public static class ReplayUtils
    {
        static readonly Action DoNothing = () => { };

        public static bool PromptConfirmReplayCompatibility(ReplayMetadata replayMeta, Action onCancel = null)
        {
            if (onCancel == null)
                onCancel = DoNothing;

            if (replayMeta == null)
            {
                //ConfirmationDialogs.ButtonPrompt("Incompatible Replay", "Replay metadata could not be read.", onCancel: onCancel);
                return false;
            }

            var version = replayMeta.GameInfo.Version;
            if (version == null)
                return IncompatibleReplayDialog("unknown version", version, onCancel);

            var mod = replayMeta.GameInfo.Mod;
            if (mod == null)
                return IncompatibleReplayDialog("unknown mod", mod, onCancel);

            if (!Game.Mods.ContainsKey(mod))
                return IncompatibleReplayDialog("unavailable mod", mod, onCancel);

            if (Game.Mods[mod].Metadata.Version != version)
                return IncompatibleReplayDialog("incompatible version", version, onCancel);

            if (replayMeta.GameInfo.MapPreview.Status != MapStatus.Available)
                return IncompatibleReplayDialog("unavailable map", replayMeta.GameInfo.MapUid, onCancel);

            return true;
        }

        static bool IncompatibleReplayDialog(string type, string name, Action onCancel)
        {
            var error = "It was recorded with an " + type;
            error += string.IsNullOrEmpty(name) ? "." : ":\n{0}".F(name);

            //ConfirmationDialogs.ButtonPrompt("Incompatible Replay", error, onCancel: onCancel);

            return false;
        }
    }
}
