using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace KingdomAnnexation.Conditions
{
    public static class PlayerTraitCondition
    {
        public static bool Fearless()
        {
            return PlayerTraitConditionInternal(Hero.MainHero.GetTraitLevel(DefaultTraits.Valor));
        }

        public static bool Generous()
        {   
            return PlayerTraitConditionInternal(Hero.MainHero.GetTraitLevel(DefaultTraits.Generosity));
        }

        public static bool Cruel()
        {
            return PlayerTraitConditionInternal(Hero.MainHero.GetTraitLevel(DefaultTraits.Mercy), negativeTrait: true);
        }

        public static bool Honorable()
        {
            return PlayerTraitConditionInternal(Hero.MainHero.GetTraitLevel(DefaultTraits.Honor));
        }

        private static bool PlayerTraitConditionInternal(int traitLevel, bool negativeTrait = false)
        {
            var hasEnoughTraitLevel = negativeTrait ? traitLevel < 0 : traitLevel > 0;
            return hasEnoughTraitLevel;
        }
    }
}