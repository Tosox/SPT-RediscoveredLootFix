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
        public static event Action<ActiveSearchController, Item> OnItemRediscovered;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ActiveSearchController), nameof(ActiveSearchController.SetItemAsKnown));
        }

        [PatchPrefix]
        public static void Prefix(ActiveSearchController __instance, Item item, bool raiseEvents, out bool __state)
        {
            __state = raiseEvents && __instance._discoveredItems.Contains(item);
        }

        [PatchPostfix]
        public static void Postfix(ActiveSearchController __instance, Item item, bool __state)
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
        public static void Postfix(GridView __instance, ItemController ____itemController, UIParent ___UI)
        {
            if (____itemController.SearchController is not ActiveSearchController searchController)
                return;

            void Handler(ActiveSearchController controller, Item item)
            {
                if (controller == searchController)
                    __instance.ItemFoundHandler(item);
            }

            SetItemAsKnownPatch.OnItemRediscovered += Handler;
            ___UI.AddDisposable(() => SetItemAsKnownPatch.OnItemRediscovered -= Handler);
        }
    }
}
