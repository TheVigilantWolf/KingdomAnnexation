//// Decompiled with JetBrains decompiler
//// Type: TaleWorlds.CampaignSystem.Actions.ChangeKingdomAction
//// Assembly: TaleWorlds.CampaignSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: 038459B8-4640-4714-AE67-6181A9569366
//// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.CampaignSystem.dll

//using Helpers;
//using System.Collections.Generic;
//using System.Linq;
//using TaleWorlds.CampaignSystem.Party;
//using TaleWorlds.CampaignSystem.Party.PartyComponents;
//using TaleWorlds.CampaignSystem.Settlements;

//#nullable disable
//namespace TaleWorlds.CampaignSystem.Actions;

//public static class ChangeKingdomAction
//{
//  public const float PotentialSettlementsPerNobleEffect = 0.2f;
//  public const float NewGainedFiefsValueForKingdomConstant = 0.1f;
//  public const float LordsUnitStrengthValue = 20f;
//  public const float MercenaryUnitStrengthValue = 5f;
//  public const float MinimumNeededGoldForRecruitingMercenaries = 20000f;

//  private static void ApplyInternal(
//    Clan clan,
//    Kingdom newKingdom,
//    ChangeKingdomAction.ChangeKingdomActionDetail detail,
//    CampaignTime shouldStayInKingdomUntil,
//    int awardMultiplier = 0,
//    bool byRebellion = false,
//    bool showNotification = true)
//  {
//    Kingdom kingdom = clan.Kingdom;
//    clan.DebtToKingdom = 0;
//    if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdom || detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinAsMercenary || detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdomByDefection)
//    {
//      clan.ShouldStayInKingdomUntil = shouldStayInKingdomUntil;
//      FactionHelper.AdjustFactionStancesForClanJoiningKingdom(clan, newKingdom);
//    }
//    else
//      clan.ShouldStayInKingdomUntil = CampaignTime.Zero;
//    if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdom || detail == ChangeKingdomAction.ChangeKingdomActionDetail.CreateKingdom || detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdomByDefection)
//    {
//      if (clan.IsUnderMercenaryService)
//        EndMercenaryServiceAction.EndByDefault(clan);
//      if (kingdom != null)
//        clan.ClanLeaveKingdom(!byRebellion);
//      if (newKingdom != null && detail == ChangeKingdomAction.ChangeKingdomActionDetail.CreateKingdom)
//        ChangeRulingClanAction.Apply(newKingdom, clan);
//      clan.Kingdom = newKingdom;
//    }
//    else if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.JoinAsMercenary)
//      StartMercenaryServiceAction.ApplyByDefault(clan, newKingdom, awardMultiplier);
//    else if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveWithRebellion || detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveKingdom || detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary || detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveByClanDestruction || detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveByKingdomDestruction)
//    {
//      clan.Kingdom = (Kingdom) null;
//      bool flag = false;
//      if (clan.IsUnderMercenaryService)
//      {
//        flag = true;
//        EndMercenaryServiceAction.EndByLeavingKingdom(clan);
//      }
//      switch (detail)
//      {
//        case ChangeKingdomAction.ChangeKingdomActionDetail.LeaveKingdom:
//          using (List<Settlement>.Enumerator enumerator = new List<Settlement>((IEnumerable<Settlement>) clan.Settlements).GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              Settlement current = enumerator.Current;
//              ChangeOwnerOfSettlementAction.ApplyByLeaveFaction(kingdom.Leader, current);
//              foreach (Hero hero in new List<Hero>((IEnumerable<Hero>) current.HeroesWithoutParty))
//              {
//                if (hero.CurrentSettlement != null && hero.Clan == clan)
//                {
//                  if (hero.PartyBelongedTo != null)
//                  {
//                    LeaveSettlementAction.ApplyForParty(hero.PartyBelongedTo);
//                    EnterSettlementAction.ApplyForParty(hero.PartyBelongedTo, clan.Leader.HomeSettlement);
//                  }
//                  else
//                  {
//                    LeaveSettlementAction.ApplyForCharacterOnly(hero);
//                    EnterSettlementAction.ApplyForCharacterOnly(hero, clan.Leader.HomeSettlement);
//                  }
//                }
//              }
//            }
//            break;
//          }
//        case ChangeKingdomAction.ChangeKingdomActionDetail.LeaveWithRebellion:
//          DeclareWarAction.ApplyByRebellion((IFaction) kingdom, (IFaction) clan);
//          using (List<IFaction>.Enumerator enumerator = kingdom.FactionsAtWarWith.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              IFaction current = enumerator.Current;
//              if (current != clan && !clan.IsAtWarWith(current))
//                DeclareWarAction.ApplyByDefault((IFaction) clan, current);
//            }
//            break;
//          }
//        case ChangeKingdomAction.ChangeKingdomActionDetail.LeaveByKingdomDestruction:
//          if (flag)
//          {
//            using (List<IFaction>.Enumerator enumerator = kingdom.FactionsAtWarWith.GetEnumerator())
//            {
//              while (enumerator.MoveNext())
//              {
//                IFaction current = enumerator.Current;
//                if (clan != current && !Campaign.Current.Models.DiplomacyModel.IsAtConstantWar((IFaction) clan, current))
//                  MakePeaceAction.Apply((IFaction) clan, current);
//              }
//              break;
//            }
//          }
//          using (List<IFaction>.Enumerator enumerator = kingdom.FactionsAtWarWith.GetEnumerator())
//          {
//            while (enumerator.MoveNext())
//            {
//              IFaction current = enumerator.Current;
//              if (clan != current && !clan.GetStanceWith(current).IsAtWar)
//                DeclareWarAction.ApplyByDefault((IFaction) clan, current);
//            }
//            break;
//          }
//      }
//    }
//    if (detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary || detail == ChangeKingdomAction.ChangeKingdomActionDetail.LeaveKingdom)
//    {
//      foreach (IFaction faction in clan.FactionsAtWarWith.ToList<IFaction>())
//      {
//        if (clan != faction && !Campaign.Current.Models.DiplomacyModel.IsAtConstantWar((IFaction) clan, faction))
//        {
//          MakePeaceAction.Apply((IFaction) clan, faction);
//          FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction((IFaction) clan, faction);
//          FactionHelper.FinishAllRelatedHostileActionsOfFactionToFaction(faction, (IFaction) clan);
//        }
//      }
//      ChangeKingdomAction.CheckIfPartyIconIsDirty(clan, kingdom);
//    }
//    foreach (WarPartyComponent warPartyComponent in (List<WarPartyComponent>) clan.WarPartyComponents)
//    {
//      if (warPartyComponent.MobileParty.MapEvent == null)
//        warPartyComponent.MobileParty.SetMoveModeHold();
//    }
//    CampaignEventDispatcher.Instance.OnClanChangedKingdom(clan, kingdom, newKingdom, detail, showNotification);
//  }

//  public static void ApplyByJoinToKingdom(
//    Clan clan,
//    Kingdom newKingdom,
//    CampaignTime shouldStayInKingdomUntil = default (CampaignTime),
//    bool showNotification = true)
//  {
//    ChangeKingdomAction.ApplyInternal(clan, newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdom, shouldStayInKingdomUntil, showNotification: showNotification);
//  }

//  public static void ApplyByJoinToKingdomByDefection(
//    Clan clan,
//    Kingdom oldKingdom,
//    Kingdom newKingdom,
//    CampaignTime shouldStayInKingdomUntil = default (CampaignTime),
//    bool showNotification = true)
//  {
//    ChangeKingdomAction.ApplyInternal(clan, newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail.JoinKingdomByDefection, shouldStayInKingdomUntil, showNotification: showNotification);
//    CampaignEventDispatcher.Instance.OnClanDefected(clan, oldKingdom, newKingdom);
//  }

//  public static void ApplyByCreateKingdom(Clan clan, Kingdom newKingdom, bool showNotification = true)
//  {
//    ChangeKingdomAction.ApplyInternal(clan, newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail.CreateKingdom, CampaignTime.Zero, showNotification: showNotification);
//  }

//  public static void ApplyByLeaveByKingdomDestruction(Clan clan, bool showNotification = true)
//  {
//    ChangeKingdomAction.ApplyInternal(clan, (Kingdom) null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveByKingdomDestruction, CampaignTime.Zero, showNotification: showNotification);
//  }

//  public static void ApplyByLeaveKingdom(Clan clan, bool showNotification = true)
//  {
//    ChangeKingdomAction.ApplyInternal(clan, (Kingdom) null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveKingdom, CampaignTime.Zero, showNotification: showNotification);
//  }

//  public static void ApplyByLeaveWithRebellionAgainstKingdom(Clan clan, bool showNotification = true)
//  {
//    ChangeKingdomAction.ApplyInternal(clan, (Kingdom) null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveWithRebellion, CampaignTime.Zero, showNotification: showNotification);
//  }

//  public static void ApplyByJoinFactionAsMercenary(
//    Clan clan,
//    Kingdom newKingdom,
//    CampaignTime shouldStayInKingdomUntil = default (CampaignTime),
//    int awardMultiplier = 50,
//    bool showNotification = true)
//  {
//    ChangeKingdomAction.ApplyInternal(clan, newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail.JoinAsMercenary, shouldStayInKingdomUntil, awardMultiplier, showNotification: showNotification);
//  }

//  public static void ApplyByLeaveKingdomAsMercenary(Clan mercenaryClan, bool showNotification = true)
//  {
//    ChangeKingdomAction.ApplyInternal(mercenaryClan, (Kingdom) null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveAsMercenary, CampaignTime.Zero, showNotification: showNotification);
//  }

//  public static void ApplyByLeaveKingdomByClanDestruction(Clan clan, bool showNotification = true)
//  {
//    ChangeKingdomAction.ApplyInternal(clan, (Kingdom) null, ChangeKingdomAction.ChangeKingdomActionDetail.LeaveByClanDestruction, CampaignTime.Zero, showNotification: showNotification);
//  }

//  private static void CheckIfPartyIconIsDirty(Clan clan, Kingdom oldKingdom)
//  {
//    IFaction faction2_1 = clan.Kingdom != null ? (IFaction) clan.Kingdom : (IFaction) clan;
//    IFaction faction2_2 = (IFaction) oldKingdom ?? (IFaction) clan;
//    foreach (MobileParty mobileParty in (List<MobileParty>) MobileParty.All)
//    {
//      if (mobileParty.IsVisible && (mobileParty.Party.Owner != null && mobileParty.Party.Owner.Clan == clan || clan == Clan.PlayerClan && (!FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, faction2_1) && FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, faction2_2) || FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, faction2_1) && !FactionManager.IsAtWarAgainstFaction(mobileParty.MapFaction, faction2_2))))
//        mobileParty.Party.SetVisualAsDirty();
//    }
//    foreach (Settlement settlement in (List<Settlement>) clan.Settlements)
//      settlement.Party.SetVisualAsDirty();
//  }

//  public enum ChangeKingdomActionDetail
//  {
//    JoinAsMercenary,
//    JoinKingdom,
//    JoinKingdomByDefection,
//    LeaveKingdom,
//    LeaveWithRebellion,
//    LeaveAsMercenary,
//    LeaveByClanDestruction,
//    CreateKingdom,
//    LeaveByKingdomDestruction,
//  }
//}
