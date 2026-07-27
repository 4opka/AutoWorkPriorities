using RimWorld;
using Verse;
using UnityEngine;

namespace AutoWorkPriorities
{
    public class Settings : ModSettings
    {
        public bool autoAssignEnabled = true;
        public float autoUpdateInterval = 5f; // seconds
        public bool usePassions = true;
        public bool useTraits = true;
        public bool useAge = true;
        public bool useHealth = true;
        public int primaryJobCount = 3;
        public int secondaryJobCount = 3;
        public int minPriority = 1;

        public void DoSettingsWindowContents(Rect rect)
        {
            var listing = new Listing_Standard();
            listing.Begin(rect);

            listing.CheckboxLabeled("AutoWorkPriorities.AutoAssign".Translate(), ref autoAssignEnabled);
            listing.Label("AutoWorkPriorities.AutoUpdateInterval".Translate() + ": " + autoUpdateInterval.ToString("F1"));
            autoUpdateInterval = listing.Slider(autoUpdateInterval, 1f, 60f);

            listing.CheckboxLabeled("AutoWorkPriorities.UsePassions".Translate(), ref usePassions);
            listing.CheckboxLabeled("AutoWorkPriorities.UseTraits".Translate(), ref useTraits);
            listing.CheckboxLabeled("AutoWorkPriorities.UseAge".Translate(), ref useAge);
            listing.CheckboxLabeled("AutoWorkPriorities.UseHealth".Translate(), ref useHealth);

            listing.Label("AutoWorkPriorities.PrimaryJobCount".Translate() + ": " + primaryJobCount);
            primaryJobCount = (int)listing.Slider(primaryJobCount, 1, 10);

            listing.Label("AutoWorkPriorities.SecondaryJobCount".Translate() + ": " + secondaryJobCount);
            secondaryJobCount = (int)listing.Slider(secondaryJobCount, 1, 10);

            listing.Label("AutoWorkPriorities.MinPriorityForJob".Translate() + ": " + minPriority);
            minPriority = (int)listing.Slider(minPriority, 0, 3);

            listing.End();
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref autoAssignEnabled, "autoAssignEnabled", true);
            Scribe_Values.Look(ref autoUpdateInterval, "autoUpdateInterval", 5f);
            Scribe_Values.Look(ref usePassions, "usePassions", true);
            Scribe_Values.Look(ref useTraits, "useTraits", true);
            Scribe_Values.Look(ref useAge, "useAge", true);
            Scribe_Values.Look(ref useHealth, "useHealth", true);
            Scribe_Values.Look(ref primaryJobCount, "primaryJobCount", 3);
            Scribe_Values.Look(ref secondaryJobCount, "secondaryJobCount", 3);
            Scribe_Values.Look(ref minPriority, "minPriority", 1);
        }
    }
}