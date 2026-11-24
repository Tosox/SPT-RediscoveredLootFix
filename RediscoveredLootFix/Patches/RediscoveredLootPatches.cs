using EFT;
using EFT.InventoryLogic;
using EFT.UI;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

namespace Tosox.RediscoveredLootFix.Patches
{
    internal class SetItemAsKnownPatch : ModulePatch
    {
        public static event Action<PlayerSearchControllerClass, Item> OnItemRediscovered;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(PlayerSearchControllerClass), nameof(PlayerSearchControllerClass.SetItemAsKnown));
        }

        [PatchPrefix]
        public static void Prefix(PlayerSearchControllerClass __instance, Item item, bool raiseEvents, out bool __state)
        {
            __state = raiseEvents && __instance.HashSet_2.Contains(item);
        }

        [PatchPostfix]
        public static void Postfix(PlayerSearchControllerClass __instance, Item item, bool __state)
        {
            if (__state)
                OnItemRediscovered?.Invoke(__instance, item);
        }
    }

    internal class GridViewShowPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GridView), nameof(GridView.Show));
        }

        [PatchPostfix]
        public static void Postfix(GridView __instance, TraderControllerClass ____itemController, AddViewListClass ___UI)
        {
            if (____itemController.SearchController is not PlayerSearchControllerClass searchController)
                return;

            void Handler(PlayerSearchControllerClass controller, Item item)
            {
                if (controller == searchController)
                    __instance.method_0(item);
            }

            SetItemAsKnownPatch.OnItemRediscovered += Handler;
            ___UI.AddDisposable(() => SetItemAsKnownPatch.OnItemRediscovered -= Handler);
        }
    }
}
