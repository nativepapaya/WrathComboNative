﻿using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal static partial class MNK
{
    internal static MNKGauge Gauge = GetJobGauge<MNKGauge>();
    internal static MNKOpenerLogicSL MNKOpenerSL = new();
    internal static MNKOpenerLogicLL MNKOpenerLL = new();

    internal static float GCD => GetCooldown(OriginalHook(Bootshine)).CooldownTotal;

    internal static bool BothNadisOpen => Gauge.Nadi.ToString() == "LUNAR, SOLAR";

    internal static bool SolarNadi => Gauge.Nadi == Nadi.SOLAR;

    internal static bool LunarNadi => Gauge.Nadi == Nadi.LUNAR;

    internal static int OpoOpoChakra => Gauge.BeastChakra.Count(x => x == BeastChakra.OPOOPO);

    internal static int RaptorChakra => Gauge.BeastChakra.Count(x => x == BeastChakra.RAPTOR);

    internal static int CoeurlChakra => Gauge.BeastChakra.Count(x => x == BeastChakra.COEURL);

    public static uint DetermineCoreAbility(uint actionId, bool useTrueNorthIfEnabled)
    {
        if (HasEffect(Buffs.OpoOpoForm) || HasEffect(Buffs.FormlessFist))
            return Gauge.OpoOpoFury == 0 && LevelChecked(DragonKick)
                ? DragonKick
                : OriginalHook(Bootshine);

        if (HasEffect(Buffs.RaptorForm))
            return Gauge.RaptorFury == 0 && LevelChecked(TwinSnakes)
                ? TwinSnakes
                : OriginalHook(TrueStrike);

        if (HasEffect(Buffs.CoeurlForm))
        {
            if (Gauge.CoeurlFury == 0 && LevelChecked(Demolish))
            {
                if (!OnTargetsRear() &&
                    TargetNeedsPositionals() &&
                    !HasEffect(Buffs.TrueNorth) &&
                    ActionReady(TrueNorth) &&
                    useTrueNorthIfEnabled)
                    return TrueNorth;

                return Demolish;
            }

            if (LevelChecked(SnapPunch))
            {
                if (!OnTargetsFlank() &&
                    TargetNeedsPositionals() &&
                    !HasEffect(Buffs.TrueNorth) &&
                    ActionReady(TrueNorth) &&
                    useTrueNorthIfEnabled)
                    return TrueNorth;

                return OriginalHook(SnapPunch);
            }
        }

        return actionId;
    }

    public static bool UsePerfectBalance()
    {
        if (ActionReady(PerfectBalance) && !HasEffect(Buffs.PerfectBalance) && !HasEffect(Buffs.FormlessFist))
        {
            // Odd window
            if ((JustUsed(OriginalHook(Bootshine)) || JustUsed(DragonKick)) &&
                !JustUsed(PerfectBalance, 20) &&
                HasEffect(Buffs.RiddleOfFire) && !HasEffect(Buffs.Brotherhood))
                return true;

            // Even window
            if ((JustUsed(OriginalHook(Bootshine)) || JustUsed(DragonKick)) &&
                (GetCooldownRemainingTime(Brotherhood) <= GCD * 3 || HasEffect(Buffs.Brotherhood)) &&
                (GetCooldownRemainingTime(RiddleOfFire) <= GCD * 3 || HasEffect(Buffs.RiddleOfFire)))
                return true;

            // Low level
            if ((JustUsed(OriginalHook(Bootshine)) || JustUsed(DragonKick)) &&
                ((HasEffect(Buffs.RiddleOfFire) && !LevelChecked(Brotherhood)) ||
                 !LevelChecked(RiddleOfFire)))
                return true;
        }

        return false;
    }

    #region Openers

    internal static WrathOpener Opener()
    {
        if (Config.MNK_SelectedOpener == 0 || IsEnabled(CustomComboPreset.MNK_ST_SimpleMode))
            return MNKOpenerLL;

        if (Config.MNK_SelectedOpener == 1)
            return MNKOpenerSL;

        return WrathOpener.Dummy;
    }

    internal class MNKOpenerLogicSL : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            PerfectBalance,
            TwinSnakes,
            Demolish,
            Brotherhood,
            RiddleOfFire,
            LeapingOpo,
            TheForbiddenChakra,
            RiddleOfWind,
            RisingPhoenix,
            DragonKick,
            WindsReply,
            FiresReply,
            LeapingOpo,
            PerfectBalance,
            DragonKick,
            LeapingOpo,
            DragonKick,
            ElixirBurst,
            LeapingOpo
        ];

        internal override UserData? ContentCheckConfig => Config.MNK_Balance_Content;
        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(PerfectBalance) < 2)
                return false;

            if (!ActionReady(Brotherhood))
                return false;

            if (!ActionReady(RiddleOfFire))
                return false;

            if (!ActionReady(RiddleOfWind))
                return false;

            if (Gauge.Nadi != Nadi.NONE)
                return false;

            if (Gauge.RaptorFury != 0)
                return false;

            if (Gauge.CoeurlFury != 0)
                return false;

            return true;
        }
    }

    internal class MNKOpenerLogicLL : WrathOpener
    {
        public override int MinOpenerLevel => 100;

        public override int MaxOpenerLevel => 109;

        public override List<uint> OpenerActions { get; set; } =
        [
            DragonKick,
            PerfectBalance,
            LeapingOpo,
            DragonKick,
            Brotherhood,
            RiddleOfFire,
            LeapingOpo,
            TheForbiddenChakra,
            RiddleOfWind,
            ElixirBurst,
            DragonKick,
            WindsReply,
            FiresReply,
            LeapingOpo,
            PerfectBalance,
            DragonKick,
            LeapingOpo,
            DragonKick,
            ElixirBurst,
            LeapingOpo
        ];
        internal override UserData? ContentCheckConfig => Config.MNK_Balance_Content;

        public override bool HasCooldowns()
        {
            if (GetRemainingCharges(PerfectBalance) < 2)
                return false;

            if (!ActionReady(Brotherhood))
                return false;

            if (!ActionReady(RiddleOfFire))
                return false;

            if (!ActionReady(RiddleOfWind))
                return false;

            if (Gauge.Nadi != Nadi.NONE)
                return false;

            if (Gauge.RaptorFury != 0)
                return false;

            if (Gauge.CoeurlFury != 0)
                return false;

            return true;
        }
    }

    #endregion
}