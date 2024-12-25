﻿using ECommons.DalamudServices;
using ECommons.GameHelpers;
using ECommons.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using WrathCombo.Combos.JobHelpers.Enums;
using WrathCombo.Combos.PvE;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Data;
using WrathCombo.Extensions;
using WrathCombo.Services;

namespace WrathCombo.CustomComboNS
{
    public abstract class WrathOpener
    {
        private OpenerState currentState = OpenerState.OpenerNotReady;
        private int openerStep;
        private static WrathOpener? currentOpener;

        private void UpdateOpener(Dalamud.Plugin.Services.IFramework framework)
        {
            if (!Service.IconReplacer.getIconHook.IsEnabled)
            {
                uint _ = 0;
                FullOpener(ref _);
            }
        }

        public void ProgressOpener(uint actionId)
        {
            if (actionId == CurrentOpenerAction)
            {
                OpenerStep++;
                if (OpenerStep > OpenerActions.Count)
                {
                    CurrentState = OpenerState.OpenerFinished;
                    return;
                }

                CurrentOpenerAction = OpenerActions[OpenerStep - 1];
            }
        }

        public virtual OpenerState CurrentState
        {
            get => currentState;
            set
            {
                if (value != currentState)
                {
                    currentState = value;

                    if (value == OpenerState.OpenerNotReady)
                        Svc.Log.Debug($"Opener Not Ready");

                    if (value == OpenerState.OpenerReady)
                        if (Service.Configuration.OutputOpenerLogs)
                            DuoLog.Information("Opener Now Ready");
                        else
                            Svc.Log.Debug($"Opener Now Ready");

                    if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                    {
                        if (value == OpenerState.FailedOpener)
                            if (Service.Configuration.OutputOpenerLogs)
                                DuoLog.Error($"Opener Failed at step {OpenerStep}, {CurrentOpenerAction.ActionName()}");
                            else
                                Svc.Log.Information($"Opener Failed at step {OpenerStep}, {CurrentOpenerAction.ActionName()}");

                        ResetOpener();
                    }

                    if (value == OpenerState.OpenerFinished)
                        if (Service.Configuration.OutputOpenerLogs)
                            DuoLog.Information("Opener Finished");
                        else
                            Svc.Log.Debug($"Opener Finished");
                }
            }
        }

        public virtual int OpenerStep
        {
            get => openerStep;
            set
            {
                if (value != openerStep)
                {
                    Svc.Log.Debug($"Opener Step {value}");
                    openerStep = value;
                }
            }
        }

        public abstract List<uint> OpenerActions { get; set; }

        public virtual List<int> DelayedWeaveSteps { get; set; } = new List<int>();

        public virtual List<(int[] Steps, uint NewAction, Func<bool> Condition)> SubstitutionSteps { get; set; } = new();

        public virtual List<(int[] Steps, int HoldDelay)> PrepullDelays { get; set; } = new();

        private int DelayedStep = 0;
        private DateTime DelayedAt;

        public uint CurrentOpenerAction { get; set; }

        public abstract int MinOpenerLevel { get; }

        public abstract int MaxOpenerLevel { get; }

        internal abstract UserData? ContentCheckConfig { get; }

        public bool LevelChecked => Player.Level >= MinOpenerLevel && Player.Level <= MaxOpenerLevel;

        public abstract bool HasCooldowns();

        public unsafe bool FullOpener(ref uint actionID)
        {
            bool inContent = ContentCheckConfig is UserBoolArray ? ContentCheck.IsInConfiguredContent((UserBoolArray)ContentCheckConfig, ContentCheck.ListSet.BossOnly) : ContentCheckConfig is UserInt ? ContentCheck.IsInConfiguredContent((UserInt)ContentCheckConfig, ContentCheck.ListSet.BossOnly) : false;
            if (!LevelChecked || OpenerActions.Count == 0 || !inContent)
            {
                return false;
            }

            CurrentOpener = this;

            if (CurrentState == OpenerState.OpenerNotReady)
            {
                if (HasCooldowns())
                {
                    CurrentState = OpenerState.OpenerReady;
                    OpenerStep = 1;
                    CurrentOpenerAction = OpenerActions.First();
                }
            }

            if (CurrentState == OpenerState.OpenerReady)
            {
                if (!HasCooldowns() && OpenerStep == 1)
                {
                    ResetOpener();
                    return false;
                }

                if (OpenerStep > 1 && ActionWatching.TimeSinceLastAction.TotalSeconds >= 5 && !PrepullDelays.Any(x => x.Steps.Any(y => y == OpenerStep)))
                {
                    CurrentState = OpenerState.FailedOpener;
                    return false;
                }

                while (CustomComboFunctions.GetCooldownRemainingTime(CurrentOpenerAction) > 6 && !CustomComboFunctions.HasCharges(CurrentOpenerAction))
                {
                    OpenerStep++;
                    if (OpenerStep >= OpenerActions.Count)
                        break;

                    CurrentOpenerAction = OpenerActions[OpenerStep - 1];
                }

                if (OpenerStep < OpenerActions.Count)
                {


                    actionID = CurrentOpenerAction = OpenerActions[OpenerStep - 1];

                    if (DelayedWeaveSteps.Any(x => x == OpenerStep))
                    {
                        if (!CustomComboFunctions.CanDelayedWeave())
                        {
                            actionID = 11;
                            return true;
                        }
                    }

                    foreach (var (Steps, NewAction, Condition) in SubstitutionSteps.Where(x => x.Steps.Any(y => y == OpenerStep)))
                    {
                        if (Condition())
                        {
                            CurrentOpenerAction = actionID = NewAction;
                            break;
                        }
                        else
                            CurrentOpenerAction = OpenerActions[OpenerStep - 1];
                    }

                    foreach (var (Steps, HoldDelay) in PrepullDelays.Where(x => x.Steps.Any(y => y == OpenerStep)))
                    {
                        if (DelayedStep != OpenerStep)
                        {
                            DelayedAt = DateTime.Now;
                            DelayedStep = OpenerStep;
                        }

                        if ((DateTime.Now - DelayedAt).TotalSeconds < HoldDelay && !CustomComboFunctions.InCombat())
                        {
                            actionID = 11;
                            return true;
                        }
                    }

                    if (Player.Object.IsCasting && ActionManager.Instance()->QueuedActionId > 0)
                        ActionManager.Instance()->QueuedActionId = 0;

                    return true;
                }

            }

            return false;
        }

        public void ResetOpener()
        {
            Svc.Log.Debug($"Opener Reset");
            DelayedStep = 0;
            OpenerStep = 0;
            CurrentOpenerAction = 0;
            CurrentState = OpenerState.OpenerNotReady;
        }

        internal static void SelectOpener(uint jobId)
        {
            CurrentOpener = jobId switch
            {
                AST.JobID => AST.Opener(),
                BLM.JobID => BLM.Opener(),
                BRD.JobID => BRD.Opener(),
                DRG.JobID => DRG.Opener(),
                DNC.JobID => DNC.Opener(),
                DRK.JobID => DRK.Opener(),
                GNB.JobID => GNB.Opener(),
                MCH.JobID => MCH.Opener(),
                MNK.JobID => MNK.Opener(),
                NIN.JobID => NIN.Opener(),
                PCT.JobID => PCT.Opener(),
                PLD.JobID => PLD.Opener(),
                RDM.JobID => RDM.Opener(),
                RPR.JobID => RPR.Opener(),
                SAM.JobID => SAM.Opener(),
                SMN.JobID => SMN.Opener(),
                SCH.JobID => SCH.Opener(),
                SGE.JobID => SGE.Opener(),
                VPR.JobID => VPR.Opener(),
                WAR.JobID => WAR.Opener(),
                WHM.JobID => WHM.Opener(),
                _ => Dummy
            };
        }

        public static WrathOpener? CurrentOpener
        {
            get => currentOpener;
            set
            {
                if (currentOpener != null && currentOpener != value)
                {
                    Svc.Framework.Update -= currentOpener.UpdateOpener;
                    CustomComboFunctions.OnCastInterrupted -= RevertInterruptedCasts;
                    Svc.Log.Debug($"Removed update hook");
                }

                if (currentOpener != value)
                {
                    currentOpener = value;
                    Svc.Framework.Update += currentOpener.UpdateOpener;
                    CustomComboFunctions.OnCastInterrupted += RevertInterruptedCasts;
                }
            }
        }

        private static void RevertInterruptedCasts()
        {
            if (CurrentOpener?.CurrentState is OpenerState.OpenerReady)
            {
                if (CurrentOpener?.OpenerStep > 1)
                    CurrentOpener.OpenerStep -= 1;
            }
        }

        public static WrathOpener Dummy = new DummyOpener();
    }

    public class DummyOpener : WrathOpener
    {
        public override List<uint> OpenerActions { get; set; } = new();
        public override int MinOpenerLevel => 1;
        public override int MaxOpenerLevel => 10000;

        internal override UserData? ContentCheckConfig => null;

        public override bool HasCooldowns() => false;
    }
}
