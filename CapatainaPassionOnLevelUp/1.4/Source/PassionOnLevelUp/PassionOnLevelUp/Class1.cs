using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace PassionOnLevelUp
{

    [StaticConstructorOnStartup]
    internal class PassionOnLevelUp : Mod
    {
        public PassionOnLevelUp (ModContentPack content)
            : base(content)
        {
            new Harmony("Capataina.PassionOnLevelUp").PatchAll();
        }

    }

    [HarmonyPatch(typeof(SkillRecord), "Learn")]
    public static class GainPassion
    {
        public static bool Prefix (float xp, bool direct, ref SkillRecord __instance, ref int __state, ref Pawn ___pawn, ref SkillDef ___def)
        {
            __state = -1;
            for (int i = 0 ; i < ___pawn.skills.skills.Count ; i++)
            {
                if (___pawn.skills.skills[i].def == ___def)
                {
                    __state = ___pawn.skills.skills[i].levelInt;
                    return true;
                }
            }
            return true;
        }

        public static void Postfix (float xp, bool direct, ref SkillRecord __instance, ref int __state, ref Pawn ___pawn, ref SkillDef ___def)
        {
            if (__state == -1)
            {
                return;
            }
            for (int j = 0 ; j < ___pawn.skills.skills.Count ; j++)
            {

                if (___pawn.skills.skills[j].def != ___def)
                {
                    continue;
                }

                while (___pawn.skills.skills[j].levelInt > __state)
                {
                    Passion pawnSkillPassion = ___pawn.skills.skills[j].passion;

                    if (___pawn.skills.skills[j].passion == (Passion)4 && (double)5 > new Random(DateTime.Now.Millisecond).NextDouble())
                    {
                        IncreasePassion((Passion)5, __instance, ___pawn, ___def);
                    }

                    if (___pawn.skills.skills[j].passion == Passion.Major && (double)10 > new Random(DateTime.Now.Millisecond).NextDouble())
                    {
                        IncreasePassion((Passion)4, __instance, ___pawn, ___def);
                    }

                    if (___pawn.skills.skills[j].passion == Passion.Minor && (double)15 > new Random(DateTime.Now.Millisecond).NextDouble())
                    {
                        IncreasePassion(Passion.Major, __instance, ___pawn, ___def);
                    }

                    if (pawnSkillPassion == Passion.None && (double)20 > new Random(DateTime.Now.Millisecond).NextDouble())
                    {
                        IncreasePassion(Passion.Minor, __instance, ___pawn, ___def);
                    }

                    if (___pawn.skills.skills[j].passion == (Passion)3 && (double)25 > new Random(DateTime.Now.Millisecond).NextDouble())
                    {
                        IncreasePassion(Passion.None, __instance, ___pawn, ___def);
                    }
                    __state++;
                }
            }
        }

        private static void IncreasePassion (Passion newPassion, SkillRecord __instance, Pawn ___pawn, SkillDef ___def)
        {
            VSE.Passions.LearnRateFactorCache.ClearCacheFor(__instance, newPassion);
            __instance.passion = newPassion;
            if (PawnUtility.ShouldSendNotificationAbout(___pawn))
            {
                Messages.Message(string.Concat(___pawn.NameShortColored, " gained interested passion for ", ___def.defName, " skill."), ___pawn, MessageTypeDefOf.PositiveEvent);
            }
        }

    }



}