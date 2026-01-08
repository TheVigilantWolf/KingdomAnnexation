using System.Collections.Generic;
using System.Linq;
using KingdomAnnexation.Actions.KingdomAnnexation;
using KingdomAnnexation.Data;
using KingdomAnnexation.Extensions;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace KingdomAnnexation
{
    public class Cheats
    {
        private const string VassalizeAllRebelsInfo =
            "Ensure you are using no spaces or caps: 'annexation.vassalize_all_rebels [kingdom]'.";

        private const string VassalizeClanInfo =
            "Ensure you are using no spaces or caps: 'annexation.vassalize_clan [kingdom] [clan]'.";

        private const string AnnexInfo =
            "Ensure you are using no spaces or caps: 'annexation.annex [annexingkingdom] [annexedkingdom]'.";

        [CommandLineFunctionality.CommandLineArgumentFunction("annex", "annexation")]
        [UsedImplicitly]
        public static string Annex(List<string> strings)
        {
            if (strings.Count < 2)
            {
                return AnnexInfo;
            }

            var annexingKingdom = KingdomExtensions.AllActiveKingdomsFactions().Find(k =>
                k.Name.ToString().ToLower().Replace(" ", "") == strings[0]
            );
            if (annexingKingdom == null)
            {
                return $"Couldn't find annexing kingdom with {strings[0]} name. {AnnexInfo}";
            }

            var annexedKingdom = KingdomExtensions.AllActiveKingdomsFactions().Find(k =>
                k.Name.ToString().ToLower().Replace(" ", "") == strings[1]
            );
            if (annexedKingdom == null)
            {
                return $"Couldn't find annexed kingdom with {strings[1]} name. {AnnexInfo}";
            }

            if (annexedKingdom == annexingKingdom)
            {
                return "Annexing kingdom and annexed kingdom are the same.";
            }

            KingdomAnnexationAction.ApplyForce(annexedKingdom, annexingKingdom, showNotification: false);
            return $"{annexingKingdom.Name} annexed {annexedKingdom.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("vassalize_all_rebels", "annexation")]
        [UsedImplicitly]
        public static string VassalizeAllRebels(List<string> strings)
        {
            if (strings.IsEmpty())
            {
                return VassalizeAllRebelsInfo;
            }

            var kingdom = KingdomExtensions.AllActiveKingdomsFactions().Find(k =>
                k.Name.ToString().ToLower().Replace(" ", "") == strings[0]);
            if (kingdom == null)
            {
                return $"Couldn't find kingdom with {strings[0]} name. {VassalizeAllRebelsInfo}";
            }

            var kingdomlessClans =
                Clan.All.Where(c =>
                        !c.IsEliminated &&
                        AnnexationRebelClansStorage.Instance?.IsRebelClanAgainstAnnexingKingdom(c, kingdom) == true
                    )
                    .ToList();
            foreach (var clan in kingdomlessClans)
            {
                if (clan.GetStanceWith(kingdom).IsAtWar)
                {
                    MakePeaceAction.Apply(clan, kingdom);
                }
                ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom);
            }

            return $"{kingdomlessClans.Count} clans without kingdom joined {kingdom.Name}.";
        }

        [CommandLineFunctionality.CommandLineArgumentFunction("vassalize_clan", "annexation")]
        [UsedImplicitly]
        public static string VassalizeClan(List<string> strings)
        {
            if (strings.Count < 2)
            {
                return VassalizeClanInfo;
            }

            var kingdom = KingdomExtensions.AllActiveKingdomsFactions().Find(k =>
                k.Name.ToString().ToLower().Replace(" ", "") == strings[0]);
            if (kingdom == null)
            {
                return $"Couldn't find kingdom with {strings[0]} name. {VassalizeClanInfo}";
            }

            var clan = Clan.All.ToList().Find(c =>
                c.Name.ToString().ToLower().Replace(" ", "") == strings[1]);
            if (clan == null)
            {
                return $"Couldn't find clan with {strings[1]} name. {VassalizeClanInfo}";
            }

            if (clan.GetStanceWith(kingdom).IsAtWar)
            {
                MakePeaceAction.Apply(clan, kingdom);
            }
            ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom);
            return $"{clan.Name} clan joined {kingdom.Name}.";
        }
    }
}