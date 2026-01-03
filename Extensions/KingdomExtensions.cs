using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;

namespace KingdomAnnexation.Extensions
{
    public static class KingdomExtensions
    {
        public static List<Kingdom> AllActiveKingdomsFactions()
        {
            return Kingdom.All.Where((kingdom) => !kingdom.IsEliminated).ToList();
        }

        public static List<Clan> VassalClans(this Kingdom kingdom)
        {
            return kingdom.Clans.Where(clan => clan.IsNoble && clan != kingdom.RulingClan && !clan.IsEliminated).ToList();
        }

        public static int KingdomsStrengthRatio(Kingdom kingdom, Kingdom other)
        {
            var otherStrength = other.CurrentTotalStrength;
            if (otherStrength <= 0f)
            {
                return 0;
            }

            var strengthRatio = (int)(kingdom.CurrentTotalStrength * 100f / otherStrength);
            return strengthRatio;
        }
    }
}