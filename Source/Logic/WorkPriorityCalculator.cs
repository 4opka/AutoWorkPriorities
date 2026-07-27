using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;

namespace AutoWorkPriorities
{
    public static class WorkPriorityCalculator
    {
        public static void CalculatePriorities(Pawn pawn, PawnData data)
        {
            var settings = ModAutoWorkPriorities.Settings;
            var workTypes = DefDatabase<WorkTypeDef>.AllDefsListForReading
                .Where(w => w.workTags != WorkTags.None && pawn.workSettings?.EverWork(w) != false)
                .ToList();

            var scores = new Dictionary<WorkTypeDef, float>();

            foreach (var workType in workTypes)
            {
                float score = 0f;

                // 1. Базовый навык (уровень)
                var skillDef = workType.workTags.GetSkill(); // нет прямого метода, нужно получить по WorkTypeDef
                // Используем extension: workType.skills
                var relevantSkills = workType.workTags.GetSkills();
                float skillLevel = 0f;
                int skillsCount = 0;
                foreach (var skill in relevantSkills)
                {
                    var pawnSkill = pawn.skills.GetSkill(skill);
                    if (pawnSkill != null)
                    {
                        skillLevel += pawnSkill.Level;
                        skillsCount++;
                    }
                }
                if (skillsCount > 0) skillLevel /= skillsCount;
                score += skillLevel * 1f; // базовый вес

                // 2. Страсть
                if (settings.usePassions)
                {
                    foreach (var skill in relevantSkills)
                    {
                        var pawnSkill = pawn.skills.GetSkill(skill);
                        if (pawnSkill != null && pawnSkill.passion != Passion.None)
                        {
                            float passionBonus = pawnSkill.passion == Passion.Major ? 2f : 1f;
                            score += passionBonus * 2f;
                        }
                    }
                }

                // 3. Черты
                if (settings.useTraits)
                {
                    foreach (var trait in pawn.story.traits.allTraits)
                    {
                        // Пример: трудолюбивый даёт бонус ко всем работам
                        if (trait.def.defName == "Industrious") score += 1f;
                        if (trait.def.defName == "Workaholic") score += 2f;
                        // Также можно учитывать черты, которые дают бонусы к конкретным работам
                        // (например, "Miner" даёт бонус к Mining)
                        // Здесь мы упрощённо даём бонус, но можно расширить
                        if (trait.def.defName == "MiningExpert") score += workType.workTags.Has(WorkTags.Mining) ? 3f : 0f;
                    }
                }

                // 4. Возраст
                if (settings.useAge)
                {
                    float age = pawn.ageTracker.AgeBiologicalYearsFloat;
                    if (age > 50) score -= (age - 50) * 0.1f; // с возрастом снижение
                    if (age < 20) score += (20 - age) * 0.1f; // молодые лучше обучаются?
                }

                // 5. Здоровье
                if (settings.useHealth)
                {
                    // Сознание
                    float consciousness = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness);
                    score *= consciousness; // если низкое, всё хуже

                    // Манипуляция
                    float manipulation = pawn.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation);
                    // Для работ требующих рук
                    if (workType.workTags.Has(WorkTags.Manipulation))
                    {
                        score *= manipulation;
                    }

                    // Инвалидность - учёт конкретных частей тела
                    // Можно учесть через Hediff, но упростим через общие ёмкости
                }

                // Учитываем ограничения: если работа требует рук, а манипуляция слишком низкая, снижаем
                if (workType.workTags.Has(WorkTags.Manipulation) && pawn.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation) < 0.3f)
                    score = 0f;

                // Добавляем в словарь
                scores[workType] = score;
            }

            // Сортируем по убыванию
            var sorted = scores.OrderByDescending(kvp => kvp.Value).ToList();

            // Определяем основные и второстепенные работы
            data.WorkScores = scores;
            data.PrimaryJobs = sorted.Take(settings.primaryJobCount).Select(kvp => kvp.Key).ToList();
            data.SecondaryJobs = sorted.Skip(settings.primaryJobCount).Take(settings.secondaryJobCount).Select(kvp => kvp.Key).ToList();

            // Применяем приоритеты
            ApplyPriorities(pawn, data, settings);
        }

        private static void ApplyPriorities(Pawn pawn, PawnData data, Settings settings)
        {
            var workSettings = pawn.workSettings;
            if (workSettings == null) return;

            // Сбрасываем все приоритеты на минимальный (0 = отключено, если разрешено)
            // Но обычно 3 - это низкий приоритет. Мы будем использовать minPriority как минимальный, который ставим.
            // Все работы, которые не вошли в основные или второстепенные, получают minPriority.
            // Можно также дать 0, если не хотим, чтобы они работали.
            // В настройках пользователь может задать minPriority.

            // Сначала всем ставим minPriority
            foreach (var workType in DefDatabase<WorkTypeDef>.AllDefsListForReading)
            {
                if (workSettings.EverWork(workType))
                {
                    workSettings.SetPriority(workType, settings.minPriority);
                }
            }

            // Основные - приоритет 1
            foreach (var workType in data.PrimaryJobs)
            {
                if (workSettings.EverWork(workType))
                {
                    workSettings.SetPriority(workType, 1);
                }
            }

            // Второстепенные - приоритет 2 (если не заняты основными)
            foreach (var workType in data.SecondaryJobs)
            {
                if (workSettings.EverWork(workType) && !data.PrimaryJobs.Contains(workType))
                {
                    workSettings.SetPriority(workType, 2);
                }
            }

            // Если minPriority >= 2, то второстепенные тоже получают 2, но мы уже поставили 1 для основных.
            // Можно дополнительно подкорректировать.

            data.UpdateVersion();
        }
    }
}