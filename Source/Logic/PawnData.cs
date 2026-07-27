using RimWorld;
using Verse;
using System.Collections.Generic;

namespace AutoWorkPriorities
{
    public class PawnData
    {
        public Pawn Pawn;
        public int Version;
        public Dictionary<WorkTypeDef, float> WorkScores;
        public List<WorkTypeDef> PrimaryJobs;
        public List<WorkTypeDef> SecondaryJobs;

        public PawnData(Pawn pawn)
        {
            Pawn = pawn;
            Version = 0;
            WorkScores = new Dictionary<WorkTypeDef, float>();
            PrimaryJobs = new List<WorkTypeDef>();
            SecondaryJobs = new List<WorkTypeDef>();
        }

        public bool IsStale()
        {
            // Проверяем изменились ли навыки, здоровье, черты и т.п.
            var currentVersion = Pawn.GetHashCode() ^
                (Pawn.skills?.skills?.Count ?? 0) ^
                (Pawn.story?.traits?.traits?.Count ?? 0) ^
                (Pawn.health?.hediffSet?.hediffs?.Count ?? 0);
            return Version != currentVersion;
        }

        public void UpdateVersion()
        {
            Version = Pawn.GetHashCode() ^
                (Pawn.skills?.skills?.Count ?? 0) ^
                (Pawn.story?.traits?.traits?.Count ?? 0) ^
                (Pawn.health?.hediffSet?.hediffs?.Count ?? 0);
        }
    }
}