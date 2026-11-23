using BepInEx;
using SPT.Reflection.Patching;
using Tosox.RediscoveredLootFix.Patches;

namespace Tosox.RediscoveredLootFix
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private readonly ModulePatch[] _patches =
        [
            new SetItemAsKnownPatch(),
            new GridViewShowPatch()
        ];

        internal void Awake()
        {
            foreach (var patch in _patches)
                patch.Enable();

            Logger.LogInfo("Plugin loaded successfully");
        }

        internal void OnDestroy()
        {
            foreach (var patch in _patches)
                patch.Disable();
        }
    }
}
