using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools
{
    [HarmonyPatch(typeof(Game), "SetFreeCamMode")]
    public class FreecamPatch
    {
        [HarmonyPrefix]
        public static void Prefix_Awake(ref bool val)
        {
       //     val = false;           
        }
    }
}
