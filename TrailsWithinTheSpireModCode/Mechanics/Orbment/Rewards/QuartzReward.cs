using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace TrailsWithinTheSpireMod.TrailsWithinTheSpireModCode.Mechanics.Orbment.Rewards;

public sealed class QuartzReward : CustomReward
{
    public static readonly RewardType QuartzRewardType = (RewardType)7001;

    public string QuartzId { get; private set; }

    private QuartzDefinition Quartz =>
        QuartzDatabase.GetById(QuartzId) ??
        QuartzDatabase.All.First();

    public QuartzReward(Player player, string quartzId) : base(player)
    {
        QuartzRewardRegistry.Register();

        if (QuartzDatabase.GetById(quartzId) == null)
        {
            GD.PrintErr($"QUARTZ_REWARD_LOG: Tried to create QuartzReward with unknown Quartz '{quartzId}'. Falling back to first Quartz.");
            QuartzId = QuartzDatabase.All.First().Id;
        }
        else
        {
            QuartzId = quartzId;
        }
    }

    protected override RewardType RewardType => QuartzRewardType;

    public override bool IsPopulated => true;

    public override LocString Description =>
        new LocString("cards", $"{Quartz.Id}.title");

    protected override string IconPath => Quartz.IconPath;

    public override IEnumerable<IHoverTip> HoverTips
    {
        get
        {
            return new IHoverTip[]
            {
                new HoverTip(
                    new LocString("cards", $"{Quartz.Id}.title"),
                    new LocString("cards", $"{Quartz.Id}.description")
                )
            };
        }
    }

    public override CreateRewardFromSave<CustomReward> DeserializeMethod => CreateFromSerializable;

    public override void Populate()
    {
        // Nothing to populate. The QuartzId is already fixed when this reward is created.
    }

    public override void MarkContentAsSeen()
    {
        // Quartz are not base-game cards/relics, so there is no SaveManager "seen" content to mark here.
    }

    public override Control CreateIcon()
    {
        var icon = new TextureRect
        {
            Name = "QuartzRewardIcon",
            Texture = GD.Load<Texture2D>(Quartz.IconPath),
            CustomMinimumSize = new Vector2(64f, 64f),
            Size = new Vector2(64f, 64f),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        return icon;
    }

    protected override Task<bool> OnSelect()
    {
        OrbmentManager.AddQuartz(Quartz.Id);

        GD.Print($"QUARTZ_REWARD_LOG: Claimed Quartz reward '{Quartz.Id}'.");

        return Task.FromResult(true);
    }

    public override SerializableReward ToSerializable()
    {
        return new SerializableReward
        {
            RewardType = QuartzRewardType,
            GoldAmount = Quartz.RewardSaveId
        };
    }

    public static CustomReward CreateFromSerializable(SerializableReward save, Player player)
    {
        QuartzRewardRegistry.Register();

        var quartz = QuartzDatabase.GetByRewardSaveId(save.GoldAmount);

        if (quartz == null)
        {
            GD.PrintErr($"QUARTZ_REWARD_LOG: Could not deserialize QuartzReward with RewardSaveId {save.GoldAmount}. Falling back to first Quartz.");
            quartz = QuartzDatabase.All.First();
        }

        return new QuartzReward(player, quartz.Id);
    }
}