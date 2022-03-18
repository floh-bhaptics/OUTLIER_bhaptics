using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;

namespace OUTLIER_bhaptics
{
    public class OUTLIER_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        #region Health
        
        [HarmonyPatch(typeof(Health), "GetCurrentValue")]
        public class bhaptics_GetHealth
        {
            [HarmonyPostfix]
            public static void Postfix(Health __instance)
            {
                if (__instance.curHealth <= 0.2f * __instance.baseMaxHealth) tactsuitVr.StartHeartBeat();
                else tactsuitVr.StopHeartBeat();
            }
        }
        
        [HarmonyPatch(typeof(Player), "Die", new Type[] {  })]
        public class bhaptics_Death
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopThreads();
            }
        }
        
        [HarmonyPatch(typeof(Health), "Heal", new Type[] { typeof(float) })]
        public class bhaptics_Heal
        {
            [HarmonyPostfix]
            public static void Postfix(Health __instance)
            {
                tactsuitVr.PlaybackHaptics("Healing");
                if (__instance.curHealth >= 0.2f * __instance.baseMaxHealth) tactsuitVr.StopHeartBeat();
            }
        }
        
        #endregion
        
        [HarmonyPatch(typeof(ShootFromInput), "MakeShoot", new Type[] { typeof(bool) })]
        public class bhaptics_Shoot
        {
            [HarmonyPostfix]
            public static void Postfix(ShootFromInput __instance)
            {
                tactsuitVr.Recoil("Gun", (__instance.hand == HandType.Right));
            }
        }

        [HarmonyPatch(typeof(DashDetector), "SuccessCast", new Type[] {  })]
        public class bhaptics_Dash
        {
            [HarmonyPostfix]
            public static void Postfix(DashDetector __instance)
            {
                if (!__instance.isDashing) return;
                tactsuitVr.PlaybackHaptics("SwooshUp");
            }
        }

        [HarmonyPatch(typeof(DamageUI), "ShowUI", new Type[] { typeof(DamageEventStatus) })]
        public class bhaptics_ShowPain
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("Impact");
            }
        }

        /*
        [HarmonyPatch(typeof(EffectManager), "AddInflictionEffect", new Type[] { typeof(TargetHitInfo), typeof(DamageType) })]
        public class bhaptics_OnHit
        {
            [HarmonyPostfix]
            public static void Postfix(EffectManager __instance, TargetHitInfo _info, DamageType _settingsType)
            {
                if (__instance == null) { tactsuitVr.LOG("Instance"); return; }
                if (_info == null) { tactsuitVr.LOG("THI"); return; }
                if (_info.hits == null) { tactsuitVr.LOG("THI.Hits"); return; }
                if (__instance.transform == null) { tactsuitVr.LOG("Instance transform"); return; }
                if (_settingsType == DamageType.Pure) return;
                try
                {
                    foreach (HitInfo myHit in _info.hits)
                    {
                        if (myHit == null) { tactsuitVr.LOG("myHit"); return; }
                        //tactsuitVr.LOG("Hit: " + __instance.settings.type.ToString());
                        tactsuitVr.LOG("Hitpoint " + myHit.point.x.ToString() + " " + __instance.transform.position.x.ToString());
                    }

                }
                catch (Exception) { }
                
                tactsuitVr.PlaybackHaptics("Impact");
            }
        }
        */

    }
}
