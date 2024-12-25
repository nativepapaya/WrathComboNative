﻿using Dalamud.Game.ClientState.JobGauge.Types;
using System;
using System.Collections.Generic;
using WrathCombo.CustomComboNS;
using WrathCombo.CustomComboNS.Functions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE
{
    internal static partial class SCH
    {
        // Class Gauge
        internal static SCHGauge Gauge => GetJobGauge<SCHGauge>();
        internal static bool HasAetherflow(this SCHGauge gauge) => (gauge.Aetherflow > 0);

        internal static SCHOpenerMaxLevel1 Opener1 = new();
        internal static WrathOpener Opener()
        {
            if (Opener1.LevelChecked) return Opener1;

            return WrathOpener.Dummy;
        }

        public static int GetMatchingConfigST(int i, out uint action, out bool enabled)
        {
            switch (i)
            {
                case 0:
                    action = Lustrate;
                    enabled = IsEnabled(CustomComboPreset.SCH_ST_Heal_Lustrate) && Gauge.HasAetherflow();
                    return Config.SCH_ST_Heal_LustrateOption;
                case 1:
                    action = Excogitation;
                    enabled = IsEnabled(CustomComboPreset.SCH_ST_Heal_Excogitation) && (Gauge.HasAetherflow() || HasEffect(Buffs.Recitation));
                    return Config.SCH_ST_Heal_ExcogitationOption;
                case 2:
                    action = Protraction;
                    enabled = IsEnabled(CustomComboPreset.SCH_ST_Heal_Protraction);
                    return Config.SCH_ST_Heal_ProtractionOption;
                case 3:
                    action = Aetherpact;
                    enabled = IsEnabled(CustomComboPreset.SCH_ST_Heal_Aetherpact) && Gauge.FairyGauge >= Config.SCH_ST_Heal_AetherpactFairyGauge && IsOriginal(Aetherpact);
                    return Config.SCH_ST_Heal_AetherpactOption;
            }

            enabled = false;
            action = 0;
            return 0;
        }

        public static int GetMatchingConfigAoE(int i, out uint action, out bool enabled)
        {
            switch (i)
            {
                case 0:
                    action = OriginalHook(WhisperingDawn);
                    enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal_WhisperingDawn);
                    return Config.SCH_AoE_Heal_WhisperingDawnOption;
                case 1:
                    action = OriginalHook(FeyIllumination);
                    enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal_FeyIllumination);
                    return Config.SCH_AoE_Heal_FeyIlluminationOption;
                case 2:
                    action = FeyBlessing;
                    enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal_FeyBlessing);
                    return Config.SCH_AoE_Heal_FeyBlessingOption;
                case 3:
                    action = Consolation;
                    enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal_Consolation) && Gauge.SeraphTimer > 0;
                    return Config.SCH_AoE_Heal_ConsolationOption;
                case 4:
                    action = Seraphism;
                    enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal_Seraphism);
                    return Config.SCH_AoE_Heal_SeraphismOption;
                case 5:
                    action = Indomitability;
                    enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal_Indomitability) && Gauge.HasAetherflow();
                    return Config.SCH_AoE_Heal_IndomitabilityOption;
                case 6:
                    action = OriginalHook(Succor);
                    enabled = IsEnabled(CustomComboPreset.SCH_AoE_Heal) && GetPartyBuffPercent(Buffs.Galvanize) <= Config.SCH_AoE_Heal_SuccorShieldOption;
                    return 100; //Don't HP Check
            }

            enabled = false;
            action = 0;
            return 0;
        }

        internal class SCHOpenerMaxLevel1 : WrathOpener
        {
            public override List<uint> OpenerActions { get; set; } =
            [
                Broil4,
                Biolysis,
                Dissipation,
                Broil4,
                ChainStratagem,
                Broil4,
                EnergyDrain,
                Broil4,
                EnergyDrain,
                Broil4,
                EnergyDrain,
                Broil4,
                Aetherflow,
                Broil4,
                BanefulImpaction,
                Broil4,
                EnergyDrain,
                Broil4,
                EnergyDrain,
                Broil4,
                EnergyDrain,
                Biolysis
            ];

            public override List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } = 
            [
                ([3], Aetherflow, () => Config.SCH_ST_DPS_OpenerOption == 1),
                ([13], Dissipation, () => Config.SCH_ST_DPS_OpenerOption == 1),
            ];

            public override int MinOpenerLevel => 100;
            public override int MaxOpenerLevel => 109;

            internal override UserData? ContentCheckConfig => Config.SCH_ST_DPS_OpenerContent;

            public override bool HasCooldowns()
            {
                if (!ActionsReady([ChainStratagem, Dissipation, Aetherflow]))
                    return false;

                return true;
            }
        }
    }
}
