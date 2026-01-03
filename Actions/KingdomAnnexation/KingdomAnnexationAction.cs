using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using KingdomAnnexation.Data;
using KingdomAnnexation.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace KingdomAnnexation.Actions.KingdomAnnexation
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static class KingdomAnnexationAction
    {
        public static void ApplyByPlayerConversation()
        {
            var annexedKingdom = Hero.OneToOneConversationHero?.Clan?.Kingdom;
            var annexingKingdom = Hero.MainHero?.Clan?.Kingdom;
            if (annexingKingdom == null || annexedKingdom == null) return;
            Apply(annexedKingdom, annexingKingdom);
        }

        public static void Apply(Kingdom annexedKingdom, Kingdom annexingKingdom, bool showNotification = true)
        {
            var annexingLeader = annexingKingdom.Leader;
            List<Clan> annexedClans = new List<Clan>(annexedKingdom.Clans);
            List<Clan> vassalClans = new List<Clan>(annexedKingdom.VassalClans());
            var clansCollaborationChance = ClansCollaborationChanceCalculator.GetChance(
                annexedKingdom: annexedKingdom,
                annexingKingdom: annexingKingdom
            );
            if (annexingKingdom.Leader == null ||
                annexingKingdom.RulingClan == null ||
                annexedKingdom.Leader == null ||
                annexedKingdom.RulingClan == null) return;
            var orderedVassalClans = vassalClans.OrderByDescending(
                clan => clan.GetRelationWithClan(annexingKingdom.RulingClan) +
                        clan.GetRelationWithClan(annexedKingdom.RulingClan)
            ).ToList();
            var clansCount = orderedVassalClans.Count;
            var collaboratingClansCount = Math.Max(1, clansCount * clansCollaborationChance / 100);
            var oldRulerName = annexedKingdom.Leader.Name;

            HandleCollaboratorsAndRebels(
                annexingKingdom,
                clansCount,
                orderedVassalClans,
                collaboratingClansCount
            );
            HandleNonVassalClans(annexedKingdom, annexingKingdom, showNotification, annexedClans);
            MakePeace(annexedKingdom, annexingKingdom);
            DestroyOldKingdom(annexedKingdom, annexingKingdom, annexingLeader);
            if (showNotification)
            {
                MBInformationManager.AddQuickInformation(new TextObject(
                    $"{annexingKingdom} annexed {annexedKingdom}. {oldRulerName} and {collaboratingClansCount} out of {vassalClans.Count} vassal clans decided to collaborate.")
                );
            }
        }

        public static void ApplyForce(Kingdom annexedKingdom, Kingdom annexingKingdom, bool showNotification = true)
        {
            var annexingLeader = annexingKingdom.Leader;
            List<Clan> annexedClans = new List<Clan>(annexedKingdom.Clans);
            List<Clan> vassalClans = new List<Clan>(annexedKingdom.VassalClans());

            if (annexingKingdom.Leader == null ||
                annexingKingdom.RulingClan == null ||
                annexedKingdom.Leader == null ||
                annexedKingdom.RulingClan == null) return;

            var oldRulerName = annexedKingdom.Leader.Name;

            HandleAllVassalsAsCollaborators(annexingKingdom, vassalClans);
            HandleNonVassalClans(annexedKingdom, annexingKingdom, showNotification, annexedClans);
            MakePeace(annexedKingdom, annexingKingdom);
            DestroyOldKingdom(annexedKingdom, annexingKingdom, annexingLeader);

            if (showNotification)
            {
                MBInformationManager.AddQuickInformation(new TextObject(
                    $"{annexingKingdom} annexed {annexedKingdom}. {oldRulerName} and {vassalClans.Count} out of {vassalClans.Count} vassal clans decided to collaborate.")
                );
            }
        }

        private static void HandleAllVassalsAsCollaborators(Kingdom annexingKingdom, List<Clan> vassalClans)
        {
            for (var i = 0; i < vassalClans.Count; i++)
            {
                var clan = vassalClans[i];
                ChangeKingdomAction.ApplyByJoinToKingdom(
                    clan: clan,
                    newKingdom: annexingKingdom,
                    showNotification: false
                );
            }
        }

        private static void DestroyOldKingdom(Kingdom annexedKingdom, Kingdom annexingKingdom, Hero annexingLeader)
        {
            if (annexingLeader.Clan?.Kingdom != annexedKingdom)
            {
                DestroyKingdomAction.Apply(annexedKingdom);
                annexedKingdom.RulingClan = annexingKingdom.RulingClan;
            }
        }

        private static void MakePeace(IFaction annexedKingdom, IFaction annexingKingdom)
        {
            if (annexedKingdom.GetStanceWith(annexingKingdom).IsAtWar)
            {
                MakePeaceAction.Apply(annexedKingdom, annexingKingdom);
            }
        }

        private static void HandleNonVassalClans(
            Kingdom annexedKingdom,
            Kingdom annexingKingdom,
            bool showNotification,
            List<Clan> annexedClans
        )
        {
            foreach (var clan in annexedClans)
            {
                if (clan.IsClanTypeMercenary)
                {
                    ChangeKingdomAction.ApplyByLeaveKingdomAsMercenary(clan, showNotification: false);
                }
                else if (clan.IsMinorFaction)
                {
                    ChangeKingdomAction.ApplyByLeaveKingdom(clan, showNotification: false);
                }
                else
                {
                    if (clan.Equals(annexedKingdom.RulingClan))
                    {
                        if (showNotification)
                        {
                            SceneNotificationData scene = new JoinKingdomSceneNotificationItem(clan, annexingKingdom);
                            MBInformationManager.ShowSceneNotification(scene);
                        }

                        ChangeKingdomAction.ApplyByJoinToKingdom(clan, annexingKingdom, showNotification: false);
                    }
                }
            }
        }

        private static void HandleCollaboratorsAndRebels(
            Kingdom annexingKingdom,
            int clansCount,
            IReadOnlyList<Clan> orderedVassalClans,
            int collaboratingClansCount
        )
        {
            for (var index = 0; index < clansCount; index++)
            {
                var clan = orderedVassalClans[index];
                if (index < collaboratingClansCount)
                {
                    ChangeKingdomAction.ApplyByJoinToKingdom(
                        clan: clan,
                        newKingdom: annexingKingdom,
                        showNotification: false
                    );
                }
                else
                {
                    ChangeKingdomAction.ApplyByLeaveKingdom(clan, showNotification: false);
                    AnnexationRebelClansStorage.Instance?.AddAnnexationRebelClan(clan, annexingKingdom);
                    DeclareWarAction.ApplyByDefault(clan, annexingKingdom);
                }
            }
        }
    }
}