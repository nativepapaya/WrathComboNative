﻿#region

using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using WrathCombo.Combos.JobHelpers.Enums;
using WrathCombo.Data;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

#endregion

namespace WrathCombo.Combos.PvE;

internal static partial class VPR
{
    // VPR Gauge & Extensions
    internal static VPROpenerLogic VPROpener = new();

    internal static VPRGauge gauge = GetJobGauge<VPRGauge>();

    internal static float GCD => GetCooldown(OriginalHook(ReavingFangs)).CooldownTotal;

    internal static float ireCD => GetCooldownRemainingTime(SerpentsIre);

    internal static bool trueNorthReady =>
        TargetNeedsPositionals() && ActionReady(All.TrueNorth) &&
        !HasEffect(All.Buffs.TrueNorth);

    internal static bool VicewinderReady => gauge.DreadCombo == DreadCombo.Dreadwinder;

    internal static bool HuntersCoilReady => gauge.DreadCombo == DreadCombo.HuntersCoil;

    internal static bool SwiftskinsCoilReady => gauge.DreadCombo == DreadCombo.SwiftskinsCoil;

    internal static bool VicepitReady => gauge.DreadCombo == DreadCombo.PitOfDread;

    internal static bool SwiftskinsDenReady => gauge.DreadCombo == DreadCombo.SwiftskinsDen;

    internal static bool HuntersDenReady => gauge.DreadCombo == DreadCombo.HuntersDen;

    internal static bool CappedOnCoils =>
        (TraitLevelChecked(Traits.EnhancedVipersRattle) && gauge.RattlingCoilStacks > 2) ||
        (!TraitLevelChecked(Traits.EnhancedVipersRattle) && gauge.RattlingCoilStacks > 1);

    internal static bool HasRattlingCoilStack(VPRGauge Gauge) => Gauge.RattlingCoilStacks > 0;

    internal class VPROpenerLogic
    {
        private OpenerState currentState = OpenerState.PrePull;

        public uint OpenerStep;

        public uint PrePullStep;

        private static uint OpenerLevel => 100;

        public static bool LevelChecked => LocalPlayer.Level >= OpenerLevel;

        private static bool CanOpener => HasCooldowns() && LevelChecked;

        public OpenerState CurrentState
        {
            get => currentState;
            set
            {
                if (value != currentState)
                {
                    if (value == OpenerState.PrePull) Svc.Log.Debug("Entered PrePull Opener");
                    if (value == OpenerState.InOpener) OpenerStep = 1;

                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            Svc.Log.Information($"Opener Failed at step {OpenerStep}");

                        ResetOpener();
                    }
                    if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                    currentState = value;
                }
            }
        }

        private static bool HasCooldowns()
        {
            if (GetRemainingCharges(Vicewinder) < 2)
                return false;

            if (!ActionReady(SerpentsIre))
                return false;

            return true;
        }

        private bool DoPrePullSteps(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CanOpener && PrePullStep == 0) PrePullStep = 1;

            if (!HasCooldowns()) PrePullStep = 0;

            if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
            {
                if (WasLastAction(ReavingFangs) && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                else if (PrePullStep == 1) actionID = ReavingFangs;

                if (ActionWatching.CombatActions.Count > 2 && InCombat())
                    CurrentState = OpenerState.FailedOpener;

                return true;
            }
            PrePullStep = 0;

            return false;
        }

        private bool DoOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (currentState == OpenerState.InOpener)
            {
                if (WasLastAction(SerpentsIre) && OpenerStep == 1) OpenerStep++;
                else if (OpenerStep == 1) actionID = SerpentsIre;

                if (WasLastAction(SwiftskinsSting) && OpenerStep == 2) OpenerStep++;
                else if (OpenerStep == 2) actionID = SwiftskinsSting;

                if (WasLastAction(Vicewinder) && OpenerStep == 3) OpenerStep++;
                else if (OpenerStep == 3) actionID = Vicewinder;

                if (WasLastAction(HuntersCoil) && OpenerStep == 4) OpenerStep++;
                else if (OpenerStep == 4) actionID = HuntersCoil;

                if (WasLastAction(TwinfangBite) && OpenerStep == 5) OpenerStep++;
                else if (OpenerStep == 5) actionID = TwinfangBite;

                if (WasLastAction(TwinbloodBite) && OpenerStep == 6) OpenerStep++;
                else if (OpenerStep == 6) actionID = TwinbloodBite;

                if (WasLastAction(SwiftskinsCoil) && OpenerStep == 7) OpenerStep++;
                else if (OpenerStep == 7) actionID = SwiftskinsCoil;

                if (WasLastAction(TwinbloodBite) && OpenerStep == 8) OpenerStep++;
                else if (OpenerStep == 8) actionID = TwinbloodBite;

                if (WasLastAction(TwinfangBite) && OpenerStep == 9) OpenerStep++;
                else if (OpenerStep == 9) actionID = TwinfangBite;

                if (WasLastAction(Reawaken) && OpenerStep == 10) OpenerStep++;
                else if (OpenerStep == 10) actionID = Reawaken;

                if (WasLastAction(FirstGeneration) && OpenerStep == 11) OpenerStep++;
                else if (OpenerStep == 11) actionID = FirstGeneration;

                if (WasLastAction(FirstLegacy) && OpenerStep == 12) OpenerStep++;
                else if (OpenerStep == 12) actionID = FirstLegacy;

                if (WasLastAction(SecondGeneration) && OpenerStep == 13) OpenerStep++;
                else if (OpenerStep == 13) actionID = SecondGeneration;

                if (WasLastAction(SecondLegacy) && OpenerStep == 14) OpenerStep++;
                else if (OpenerStep == 14) actionID = SecondLegacy;

                if (WasLastAction(ThirdGeneration) && OpenerStep == 15) OpenerStep++;
                else if (OpenerStep == 15) actionID = ThirdGeneration;

                if (WasLastAction(ThirdLegacy) && OpenerStep == 16) OpenerStep++;
                else if (OpenerStep == 16) actionID = ThirdLegacy;

                if (WasLastAction(FourthGeneration) && OpenerStep == 17) OpenerStep++;
                else if (OpenerStep == 17) actionID = FourthGeneration;

                if (WasLastAction(FourthLegacy) && OpenerStep == 18) OpenerStep++;
                else if (OpenerStep == 18) actionID = FourthLegacy;

                if (WasLastAction(Ouroboros) && OpenerStep == 19) OpenerStep++;
                else if (OpenerStep == 19) actionID = Ouroboros;

                if (WasLastAction(UncoiledFury) && OpenerStep == 20) OpenerStep++;
                else if (OpenerStep == 20) actionID = UncoiledFury;

                if (WasLastAction(UncoiledTwinfang) && OpenerStep == 21) OpenerStep++;
                else if (OpenerStep == 21) actionID = UncoiledTwinfang;

                if (WasLastAction(UncoiledTwinblood) && OpenerStep == 22) OpenerStep++;
                else if (OpenerStep == 22) actionID = UncoiledTwinblood;

                if (WasLastAction(UncoiledFury) && OpenerStep == 23) OpenerStep++;
                else if (OpenerStep == 23) actionID = UncoiledFury;

                if (WasLastAction(UncoiledTwinfang) && OpenerStep == 24) OpenerStep++;
                else if (OpenerStep == 24) actionID = UncoiledTwinfang;

                if (WasLastAction(UncoiledTwinblood) && OpenerStep == 25) OpenerStep++;
                else if (OpenerStep == 25) actionID = UncoiledTwinblood;

                if (WasLastAction(HindstingStrike) && OpenerStep == 26) OpenerStep++;
                else if (OpenerStep == 26) actionID = HindstingStrike;

                if (WasLastAction(DeathRattle) && OpenerStep == 27) OpenerStep++;
                else if (OpenerStep == 27) actionID = DeathRattle;

                if (WasLastAction(Vicewinder) && OpenerStep == 28) OpenerStep++;
                else if (OpenerStep == 28) actionID = Vicewinder;

                if (WasLastAction(UncoiledFury) && OpenerStep == 29) OpenerStep++;
                else if (OpenerStep == 29) actionID = UncoiledFury;

                if (WasLastAction(UncoiledTwinfang) && OpenerStep == 30) OpenerStep++;
                else if (OpenerStep == 30) actionID = UncoiledTwinfang;

                if (WasLastAction(UncoiledTwinblood) && OpenerStep == 31) OpenerStep++;
                else if (OpenerStep == 31) actionID = UncoiledTwinblood;

                if (WasLastAction(HuntersCoil) && OpenerStep == 32) OpenerStep++;
                else if (OpenerStep == 32) actionID = HuntersCoil;

                if (WasLastAction(TwinfangBite) && OpenerStep == 33) OpenerStep++;
                else if (OpenerStep == 33) actionID = TwinfangBite;

                if (WasLastAction(TwinbloodBite) && OpenerStep == 34) OpenerStep++;
                else if (OpenerStep == 34) actionID = TwinbloodBite;

                if (WasLastAction(SwiftskinsCoil) && OpenerStep == 35) OpenerStep++;
                else if (OpenerStep == 35) actionID = SwiftskinsCoil;

                if (WasLastAction(TwinbloodBite) && OpenerStep == 36) OpenerStep++;
                else if (OpenerStep == 36) actionID = TwinbloodBite;

                if (WasLastAction(TwinfangBite) && OpenerStep == 37) CurrentState = OpenerState.OpenerFinished;
                else if (OpenerStep == 37) actionID = TwinfangBite;

                if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                    CurrentState = OpenerState.FailedOpener;

                if (((actionID == SerpentsIre && IsOnCooldown(SerpentsIre)) ||
                     (actionID == Vicewinder && GetRemainingCharges(Vicewinder) < 2)) &&
                    ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                {
                    CurrentState = OpenerState.FailedOpener;

                    return false;
                }

                return true;
            }

            return false;
        }

        private void ResetOpener()
        {
            PrePullStep = 0;
            OpenerStep = 0;
        }

        public bool DoFullOpener(ref uint actionID)
        {
            if (!LevelChecked)
                return false;

            if (CurrentState == OpenerState.PrePull)
                if (DoPrePullSteps(ref actionID))
                    return true;

            if (CurrentState == OpenerState.InOpener)
                if (DoOpener(ref actionID))
                    return true;

            if (!InCombat())
            {
                ResetOpener();
                CurrentState = OpenerState.PrePull;
            }

            return false;
        }
    }

    internal static class VPRHelper
    {
        internal static bool UseReawaken(VPRGauge gauge)
        {
            float ireCD = GetCooldownRemainingTime(SerpentsIre);

            if (LevelChecked(Reawaken) && !HasEffect(Buffs.Reawakened) && InActionRange(Reawaken) &&
                !HasEffect(Buffs.HuntersVenom) && !HasEffect(Buffs.SwiftskinsVenom) &&
                !HasEffect(Buffs.PoisedForTwinblood) && !HasEffect(Buffs.PoisedForTwinfang) &&
                !IsEmpowermentExpiring(6))
                if ((!JustUsed(SerpentsIre, 2.2f) && HasEffect(Buffs.ReadyToReawaken)) || //2min burst
                    (WasLastWeaponskill(Ouroboros) && gauge.SerpentOffering >= 50 && ireCD >= 50) || //2nd RA
                    (gauge.SerpentOffering is >= 50 and <= 80 && ireCD is >= 50 and <= 62) || //1min
                    gauge.SerpentOffering >= 100 || //overcap
                    (gauge.SerpentOffering >= 50 && WasLastWeaponskill(FourthGeneration) &&
                     !LevelChecked(Ouroboros))) //<100
                    return true;

            return false;
        }

        internal static bool IsHoningExpiring(float Times)
        {
            float GCD = GetCooldown(SteelFangs).CooldownTotal * Times;

            return (HasEffect(Buffs.HonedSteel) && GetBuffRemainingTime(Buffs.HonedSteel) < GCD) ||
                   (HasEffect(Buffs.HonedReavers) && GetBuffRemainingTime(Buffs.HonedReavers) < GCD);
        }

        internal static bool IsVenomExpiring(float Times)
        {
            float GCD = GetCooldown(SteelFangs).CooldownTotal * Times;

            return (HasEffect(Buffs.FlankstungVenom) && GetBuffRemainingTime(Buffs.FlankstungVenom) < GCD) ||
                   (HasEffect(Buffs.FlanksbaneVenom) && GetBuffRemainingTime(Buffs.FlanksbaneVenom) < GCD) ||
                   (HasEffect(Buffs.HindstungVenom) && GetBuffRemainingTime(Buffs.HindstungVenom) < GCD) ||
                   (HasEffect(Buffs.HindsbaneVenom) && GetBuffRemainingTime(Buffs.HindsbaneVenom) < GCD);
        }

        internal static bool IsEmpowermentExpiring(float Times)
        {
            float GCD = GetCooldown(SteelFangs).CooldownTotal * Times;

            return GetBuffRemainingTime(Buffs.Swiftscaled) < GCD || GetBuffRemainingTime(Buffs.HuntersInstinct) < GCD;
        }

        internal static unsafe bool IsComboExpiring(float Times)
        {
            float GCD = GetCooldown(SteelFangs).CooldownTotal * Times;

            return ActionManager.Instance()->Combo.Timer != 0 && ActionManager.Instance()->Combo.Timer < GCD;
        }
    }
}