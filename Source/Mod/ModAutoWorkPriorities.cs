using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace AutoWorkPriorities
{
    public class ModAutoWorkPriorities : Mod
    {
        public static Settings Settings;
        public static ModAutoWorkPriorities Instance;

        public ModAutoWorkPriorities(ModContentPack content) : base(content)
        {
            Instance = this;
            Settings = GetSettings<Settings>();
            var harmony = new Harmony("com.autoworkpriorities");
            harmony.PatchAll();
        }

        public override string SettingsCategory() => "Auto Work Priorities";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }
}