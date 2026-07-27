using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace AutoWorkPriorities
{
    [HarmonyPatch(typeof(MainTabWindow_Work))]
    public static class WorkTabPatch
    {
        private static bool autoAssignEnabled = false;
        private static float lastUpdateTime = 0f;

        [HarmonyPatch("DrawWorkTable")]
        [HarmonyPostfix]
        public static void DrawWorkTablePostfix(MainTabWindow_Work __instance, Rect rect)
        {
            // Проверяем, есть ли выбранный колонист
            var pawn = __instance.SelectedPawn;
            if (pawn == null || pawn.workSettings == null) return;

            // Рисуем кнопку и чекбокс
            var buttonRect = new Rect(rect.x + rect.width - 200, rect.y, 180, 30);
            DrawAutoAssignControls(buttonRect, pawn);

            // Автоматическое обновление по таймеру
            if (ModAutoWorkPriorities.Settings.autoAssignEnabled && autoAssignEnabled)
            {
                if (Time.realtimeSinceStartup - lastUpdateTime >= ModAutoWorkPriorities.Settings.autoUpdateInterval)
                {
                    RecalculateForPawn(pawn);
                    lastUpdateTime = Time.realtimeSinceStartup;
                }
            }
        }

        private static void DrawAutoAssignControls(Rect rect, Pawn pawn)
        {
            var rectCheck = new Rect(rect.x, rect.y, 20, 20);
            var rectLabel = new Rect(rect.x + 25, rect.y, 100, 20);
            var rectButton = new Rect(rect.x + 130, rect.y, 50, 20);

            bool previous = autoAssignEnabled;
            Widgets.Checkbox(rectCheck, ref autoAssignEnabled);
            Widgets.Label(rectLabel, "AutoWorkPriorities.AutoAssign".Translate());

            if (Widgets.ButtonText(rectButton, "AutoWorkPriorities.RecalculateNow".Translate()))
            {
                RecalculateForPawn(pawn);
            }

            if (previous != autoAssignEnabled && autoAssignEnabled)
            {
                // При включении сразу пересчитываем
                RecalculateForPawn(pawn);
                lastUpdateTime = Time.realtimeSinceStartup;
            }
        }

        private static void RecalculateForPawn(Pawn pawn)
        {
            var data = WorkPriorityCache.GetOrCreate(pawn);
            // Пересчёт уже выполняется внутри GetOrCreate, если данные устарели
            // Можно принудительно пересчитать:
            WorkPriorityCalculator.CalculatePriorities(pawn, data);
        }
    }
}