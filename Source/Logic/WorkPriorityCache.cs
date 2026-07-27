using RimWorld;
using Verse;
using System.Collections.Generic;

namespace AutoWorkPriorities
{
    public static class WorkPriorityCache
    {
        private static Dictionary<Pawn, PawnData> cache = new Dictionary<Pawn, PawnData>();

        public static PawnData GetOrCreate(Pawn pawn)
        {
            if (cache.TryGetValue(pawn, out var data))
            {
                if (data.IsStale())
                {
                    Recalculate(pawn, data);
                }
                return data;
            }
            else
            {
                var newData = new PawnData(pawn);
                Recalculate(pawn, newData);
                cache.Add(pawn, newData);
                return newData;
            }
        }

        public static void Recalculate(Pawn pawn, PawnData data)
        {
            WorkPriorityCalculator.CalculatePriorities(pawn, data);
        }

        public static void Clear()
        {
            cache.Clear();
        }

        public static void Remove(Pawn pawn)
        {
            if (cache.ContainsKey(pawn))
                cache.Remove(pawn);
        }
    }
}