using TaleWorlds.CampaignSystem;

namespace KingdomAnnexation.Extensions
{
    public static class HeroExtensions
    {
        public static bool IsRulerOfKingdom(this Hero hero)
        {
            return hero.Clan?.Kingdom?.Leader == hero;
        }
    }
}