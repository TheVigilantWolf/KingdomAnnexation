using System.Collections.Generic;
using KingdomAnnexation.CampaignBehaviors;
using KingdomAnnexation.Model;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace KingdomAnnexation
{
    [UsedImplicitly]
    public class SubModule : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (game.GameType is Campaign)
            {
                var behaviors = new List<CampaignBehaviorBase>
                {
                    new PlayerRulerAnnexationConversationCampaignBehavior(),
                    new OtherRulersAnnexationCampaignBehavior(),
                    new SyncRebelClansStorageCampaignBehavior(),
                };
                foreach (var behavior in behaviors)
                {
                    ((CampaignGameStarter)gameStarterObject).AddBehavior(behavior);
                }

                var models = new List<GameModel>
                {
                    new AnnexedKingdomRebelsExecutionRelationModel(),
                };
                foreach (var model in models)
                {
                    ((CampaignGameStarter)gameStarterObject).AddModel(model);
                }
            }
        }
    }
}