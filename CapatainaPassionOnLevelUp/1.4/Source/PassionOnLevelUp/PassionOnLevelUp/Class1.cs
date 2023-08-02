using HarmonyLib;
using RimWorld;
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
                    int randomNumber = Verse.Rand.Range(0, 100);

                    if (pawnSkillPassion == (Passion)4 && (int)5 > randomNumber)
                    {
                        IncreasePassion((Passion)5, __instance, ___pawn, ___def);
                    }

                    if (pawnSkillPassion == Passion.Major && (int)10 > randomNumber)
                    {
                        IncreasePassion((Passion)4, __instance, ___pawn, ___def);
                    }

                    if (pawnSkillPassion == Passion.Minor && (int)15 > randomNumber)
                    {
                        IncreasePassion(Passion.Major, __instance, ___pawn, ___def);
                    }

                    if (pawnSkillPassion == Passion.None && (int)20 > randomNumber)
                    {
                        IncreasePassion(Passion.Minor, __instance, ___pawn, ___def);
                    }

                    if (pawnSkillPassion == (Passion)3 && (int)25 > randomNumber)
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