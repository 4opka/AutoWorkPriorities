using HarmonyLib;
using RimWorld;
using Verse;

namespace AutoWorkPriorities
{
    public static class Patches
    {
        // Можно добавить патчи на события изменения здоровья, навыков и т.п., чтобы обновлять кэш
        // Например:
        [HarmonyPatch(typeof(SkillRecord), nameof(SkillRecord.Level), MethodType.Setter)]
        [HarmonyPostfix]
        public static void OnSkillLevelChanged(SkillRecord __instance)
        {
            var pawn = __instance.pawn;
            if (pawn != null)
            {
                WorkPriorityCache.Remove(pawn);
            }
        }

        [HarmonyPatch(typeof(HediffSet), nameof(HediffSet.AddHediff))]
        [HarmonyPostfix]
        public static void OnHediffAdded(HediffSet __instance, Hediff hediff)
        {
            var pawn = __instance.pawn;
            if (pawn != null)
            {
                WorkPriorityCache.Remove(pawn);
            }
        }
    }
}