using HarmonyLib;
using UnityEngine;
using System.Collections.Generic; // For System.Collections.Generic.List
using System.Linq;
using System;
using SOD.Common;
using Il2CppSystem.Collections.Generic;

namespace DevTools
{
    [HarmonyPatch(typeof(BioScreenController), "UpdateDecorEditButton")]
    public class BioScreenControllerPatch
    {
        // Static variables to track business locations we want to enable for decor editing
        public static System.Collections.Generic.List<NewGameLocation> businessLocations = new System.Collections.Generic.List<NewGameLocation>();

        // Prefix patch for UpdateDecorEditButton to enable decor editing for business locations
        public static bool Prefix(BioScreenController __instance)
        {
            // Skip if in floor edit mode
            if (SessionData.Instance.isFloorEdit)
            {
                return true; // Continue with original method
            }

            // Check if the player is in their home, owned apartment, or a business location we've marked for editing
            bool isHome = Player.Instance.currentGameLocation == Player.Instance.home;
            bool isOwnedApartment = false;
            
            // Check if current location is in owned apartments
            foreach (var apartment in Player.Instance.apartmentsOwned)
            {
                if (apartment == Player.Instance.currentGameLocation)
                {
                    isOwnedApartment = true;
                    break;
                }
            }
            
            bool isBusinessLocation = businessLocations.Contains(Player.Instance.currentGameLocation);
            bool isAllowed = (Game.Instance.sandboxMode || ChapterController.Instance.currentPart >= 30);
            
            if ((isHome || isOwnedApartment || isBusinessLocation) && isAllowed)
            {
                // Enable the edit decor button
                __instance.editDecorButton.gameObject.SetActive(true);
                return false; // Skip original method
            }

            // Otherwise, disable the edit decor button
            __instance.editDecorButton.gameObject.SetActive(false);
            return false; // Skip original method
        }
    }
}
