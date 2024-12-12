using System.Collections.Generic;
using WrathCombo.Combos.PvE.Content;
using WrathCombo.CustomComboNS;
using WrathCombo.Data;
using WrathCombo.Extensions;
using static WrathCombo.CustomComboNS.Functions.CustomComboFunctions;

namespace WrathCombo.Combos.PvE;

internal static partial class BLM
{
    internal class BLM_ST_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_ST_SimpleMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is not Fire)
                return actionID;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.BLM_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                canWeave)
                return Variant.VariantRampart;

            if (BLMOpener.DoFullOpener(ref actionID))
                return actionID;

            //Weaves
            if (canWeave)
            {
                if (ActionReady(Amplifier) && remainingPolyglotCD >= 20000)
                    return Amplifier;

                if (ActionReady(LeyLines) && !HasEffect(Buffs.LeyLines))
                    return LeyLines;
            }

            if (HasEffect(Buffs.Thunderhead) && gcdsInTimer > 1 && LevelChecked(Thunder) &&
                GetTargetHPPercent() >= Config.BLM_ST_ThunderHP &&
                (thunderDebuffST is null || thunderDebuffST.RemainingTime < 3))
                return OriginalHook(Thunder);

            if (IsMoving)
            {
                if (ActionReady(Amplifier) && Gauge.PolyglotStacks < maxPolyglot)
                    return Amplifier;

                if (HasPolyglotStacks(Gauge))
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;
            }

            if (Gauge.InAstralFire)
            {
                if (Gauge.IsParadoxActive && gcdsInTimer < 2 && curMp >= MP.FireI)
                    return Paradox;

                if ((HasEffect(Buffs.Firestarter) && gcdsInTimer < 2 &&
                     curMp >= MP.FireI) || (HasEffect(Buffs.Firestarter) && Gauge.AstralFireStacks < 3))
                    return Fire3;

                if (curMp < MP.FireI && LevelChecked(Despair) && curMp >= MP.Despair)
                    return Despair;

                if (curMp == 0 && LevelChecked(FlareStar) && Gauge.AstralSoulStacks == 6)
                {
                    if (canWeave && ActionReady(Triplecast) &&
                        GetBuffStacks(Buffs.Triplecast) == 0 &&
                        ActionReady(Triplecast))
                        return Triplecast;

                    if (canWeave && ActionReady(All.Swiftcast) &&
                        GetBuffStacks(Buffs.Triplecast) == 0)
                        return All.Swiftcast;

                    return FlareStar;
                }

                if (LevelChecked(Fire4))
                    if (gcdsInTimer > 1 && curMp >= MP.FireI)
                    {
                        if (canWeave && ActionReady(Triplecast) &&
                            GetBuffStacks(Buffs.Triplecast) == 0 &&
                            ActionReady(Triplecast))
                            return Triplecast;

                        if (HasEffect(Buffs.Thunderhead) && gcdsInTimer > 1 &&
                            (thunderDebuffST is null || thunderDebuffST.RemainingTime < 3))
                            return OriginalHook(Thunder);

                        if (HasPolyglotStacks(Gauge) &&
                            canWeave && ActionReady(Triplecast) &&
                            GetBuffStacks(Buffs.Triplecast) == 0 &&
                            ActionReady(Triplecast))
                            return Xenoglossy.LevelChecked()
                                ? Xenoglossy
                                : Foul;

                        return Fire4;
                    }

                if (curMp >= MP.FireI)
                    return Fire;

                if (ActionReady(Manafont))
                    return HasEffect(Buffs.Firestarter)
                        ? Fire3
                        : Manafont;

                if (ActionReady(Blizzard3) &&
                    (ActionReady(All.Swiftcast) || HasEffect(Buffs.Triplecast)))
                {
                    if (canWeave && ActionReady(Transpose))
                        return Transpose;

                    if (HasEffect(Buffs.Thunderhead) &&
                        (thunderDebuffST is null || thunderDebuffST.RemainingTime < 3))
                        return OriginalHook(Thunder);

                    if (HasPolyglotStacks(Gauge))
                        return LevelChecked(Xenoglossy)
                            ? Xenoglossy
                            : Foul;
                }

                return LevelChecked(Blizzard3)
                    ? Blizzard3
                    : Transpose;
            }

            if (Gauge.InUmbralIce)
            {
                if (ActionReady(Blizzard3) && Gauge.UmbralIceStacks < 3 && TraitLevelChecked(Traits.UmbralHeart))
                {
                    if (HasEffect(All.Buffs.Swiftcast) || HasEffect(Buffs.Triplecast))
                        return Blizzard3;

                    if (GetBuffStacks(Buffs.Triplecast) == 0 && IsOffCooldown(All.Swiftcast))
                        return All.Swiftcast;

                    if (GetBuffStacks(Buffs.Triplecast) == 0 && ActionReady(Triplecast))
                        return Triplecast;
                }

                if (LevelChecked(Blizzard4) && Gauge.UmbralHearts < 3 && TraitLevelChecked(Traits.UmbralHeart))
                    return Blizzard4;

                if (Gauge.IsParadoxActive)
                    return Paradox;

                if (HasPolyglotStacks(Gauge))
                {
                    if (!HasEffect(Buffs.Firestarter) ||
                        !(GetBuffRemainingTime(Buffs.Firestarter) <= 3))
                        return LevelChecked(Xenoglossy)
                            ? Xenoglossy
                            : Foul;

                    if (curMp + nextMpGain <= 10000 || curMp < 7500)
                        return Blizzard;

                    if (ActionReady(Transpose) && canWeave &&
                        curMp is MP.MaxMP && HasEffect(Buffs.Firestarter))
                        return Transpose;

                    if (LevelChecked(Fire3))
                        return Fire3;

                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;
                }

                if (curMp + nextMpGain >= 7500 &&
                    (LocalPlayer.CastActionId == Blizzard ||
                     WasLastSpell(Blizzard) ||
                     WasLastSpell(Blizzard4)))
                    return LevelChecked(Fire3)
                        ? Fire3
                        : Fire;

                if (curMp + nextMpGain <= 10000 || curMp < 7500)
                    return Blizzard;

                if (ActionReady(Transpose) && canWeave &&
                    curMp is MP.MaxMP && HasEffect(Buffs.Firestarter))
                    return Transpose;

                return LevelChecked(Fire3)
                    ? Fire3
                    : Transpose;
            }

            if (Blizzard3.LevelChecked())
                return Blizzard3;

            return actionID;
        }
    }

    internal class BLM_ST_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_ST_AdvancedMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            int PolyglotStacks = Gauge.PolyglotStacks;
            float TriplecastChargetime = GetCooldownChargeRemainingTime(Triplecast);

            if (actionID is not Fire)
                return actionID;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.BLM_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                canWeave)
                return Variant.VariantRampart;

            if (IsEnabled(CustomComboPreset.BLM_ST_Opener))
                if (BLMOpener.DoFullOpener(ref actionID))
                    return actionID;

            //Weaves
            if (canWeave)
            {
                if (IsEnabled(CustomComboPreset.BLM_ST_Amplifier) &&
                    ActionReady(Amplifier) && remainingPolyglotCD >= 20000)
                    return Amplifier;

                if (IsEnabled(CustomComboPreset.BLM_ST_LeyLines) &&
                    ActionReady(LeyLines) && !HasEffect(Buffs.LeyLines) &&
                    GetRemainingCharges(LeyLines) > Config.BLM_ST_LeyLinesCharges)
                    return LeyLines;
            }

            if (IsEnabled(CustomComboPreset.BLM_ST_Thunder) &&
                HasEffect(Buffs.Thunderhead) && gcdsInTimer > 1 && LevelChecked(Thunder) &&
                GetTargetHPPercent() >= Config.BLM_ST_ThunderHP &&
                (thunderDebuffST is null || thunderDebuffST.RemainingTime < 3))
                return OriginalHook(Thunder);

            if (IsMoving)
            {
                if (IsEnabled(CustomComboPreset.BLM_ST_Amplifier) &&
                    ActionReady(Amplifier) && Gauge.PolyglotStacks < maxPolyglot)
                    return Amplifier;

                if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglotMoving) &&
                    PolyglotStacks > Config.BLM_ST_UsePolyglotMoving_HoldCharges)
                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;
            }

            if (Gauge.InAstralFire)
            {
                if (Gauge.IsParadoxActive && elementTimer <= 3 && curMp >= MP.FireI)
                    return Paradox;

                if ((HasEffect(Buffs.Firestarter) && elementTimer <= 3 &&
                     curMp >= MP.FireI) || (HasEffect(Buffs.Firestarter) && Gauge.AstralFireStacks < 3))
                    return Fire3;

                if (IsEnabled(CustomComboPreset.BLM_ST_Despair) &&
                    curMp < MP.FireI && LevelChecked(Despair) && curMp >= MP.Despair)
                    return Despair;

                if (IsEnabled(CustomComboPreset.BLM_ST_FlareStar) &&
                    curMp == 0 && LevelChecked(FlareStar) && Gauge.AstralSoulStacks == 6)
                {
                    if (IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                        canWeave && ActionReady(Triplecast) &&
                        GetBuffStacks(Buffs.Triplecast) == 0 &&
                        (GetRemainingCharges(Triplecast) > Config.BLM_ST_Triplecast_HoldCharges ||
                         TriplecastChargetime <= Config.BLM_ST_Triplecast_ChargeTime))
                        return Triplecast;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Swiftcast) &&
                        canWeave && ActionReady(All.Swiftcast) &&
                        GetBuffStacks(Buffs.Triplecast) == 0)
                        return All.Swiftcast;

                    return FlareStar;
                }

                if (LevelChecked(Fire4))
                    if (elementTimer >= 3 && curMp >= MP.FireI)
                    {
                        if (IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                            canWeave && ActionReady(Triplecast) &&
                            GetBuffStacks(Buffs.Triplecast) == 0 &&
                            (GetRemainingCharges(Triplecast) > Config.BLM_ST_Triplecast_HoldCharges ||
                             TriplecastChargetime <= Config.BLM_ST_Triplecast_ChargeTime))
                            return Triplecast;

                        if (IsEnabled(CustomComboPreset.BLM_ST_Thunder) &&
                            HasEffect(Buffs.Thunderhead) && gcdsInTimer > 1 &&
                            (thunderDebuffST is null || thunderDebuffST.RemainingTime < 3))
                            return OriginalHook(Thunder);

                        if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                            PolyglotStacks > Config.BLM_ST_UsePolyglot_HoldCharges &&
                            IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                            canWeave && ActionReady(Triplecast) &&
                            GetBuffStacks(Buffs.Triplecast) == 0 &&
                            (GetRemainingCharges(Triplecast) > Config.BLM_ST_Triplecast_HoldCharges ||
                             TriplecastChargetime <= Config.BLM_ST_Triplecast_ChargeTime))
                            return Xenoglossy.LevelChecked()
                                ? Xenoglossy
                                : Foul;

                        return Fire4;
                    }

                if (curMp >= MP.FireI)
                    return Fire;

                if (IsEnabled(CustomComboPreset.BLM_ST_Manafont) &&
                    ActionReady(Manafont))
                    return HasEffect(Buffs.Firestarter)
                        ? Fire3
                        : Manafont;

                if (ActionReady(Blizzard3) &&
                    ((IsEnabled(CustomComboPreset.BLM_ST_Swiftcast) && ActionReady(All.Swiftcast)) ||
                     HasEffect(Buffs.Triplecast)))
                {
                    if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                        canWeave && ActionReady(Transpose))
                        return Transpose;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Thunder) &&
                        HasEffect(Buffs.Thunderhead) &&
                        (thunderDebuffST is null || thunderDebuffST.RemainingTime < 3))
                        return OriginalHook(Thunder);

                    if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                        PolyglotStacks > Config.BLM_ST_UsePolyglot_HoldCharges)
                        return LevelChecked(Xenoglossy)
                            ? Xenoglossy
                            : Foul;
                }

                return LevelChecked(Blizzard3)
                    ? Blizzard3
                    : Transpose;
            }

            if (Gauge.InUmbralIce)
            {
                if (ActionReady(Blizzard3) && Gauge.UmbralIceStacks < 3 && TraitLevelChecked(Traits.UmbralHeart))
                {
                    if (HasEffect(All.Buffs.Swiftcast) || HasEffect(Buffs.Triplecast))
                        return Blizzard3;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Swiftcast) &&
                        GetBuffStacks(Buffs.Triplecast) == 0 && IsOffCooldown(All.Swiftcast))
                        return All.Swiftcast;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Triplecast) &&
                        LevelChecked(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0 &&
                        (GetRemainingCharges(Triplecast) > Config.BLM_ST_Triplecast_HoldCharges ||
                         TriplecastChargetime <= Config.BLM_ST_Triplecast_ChargeTime))
                        return Triplecast;
                }

                if (LevelChecked(Blizzard4) && Gauge.UmbralHearts < 3 && TraitLevelChecked(Traits.UmbralHeart))
                    return Blizzard4;

                if (Gauge.IsParadoxActive)
                    return Paradox;

                if (IsEnabled(CustomComboPreset.BLM_ST_UsePolyglot) &&
                    PolyglotStacks > Config.BLM_ST_UsePolyglot_HoldCharges)
                {
                    if (!HasEffect(Buffs.Firestarter) ||
                        !(GetBuffRemainingTime(Buffs.Firestarter) <= 3))
                        return LevelChecked(Xenoglossy)
                            ? Xenoglossy
                            : Foul;

                    if (curMp + nextMpGain <= 10000 || curMp < 7500)
                        return Blizzard;

                    if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                        ActionReady(Transpose) && canWeave &&
                        curMp is MP.MaxMP && HasEffect(Buffs.Firestarter))
                        return Transpose;

                    if (LevelChecked(Fire3))
                        return Fire3;

                    return LevelChecked(Xenoglossy)
                        ? Xenoglossy
                        : Foul;
                }

                if (curMp + nextMpGain >= 7500 &&
                    (LocalPlayer.CastActionId == Blizzard ||
                     WasLastSpell(Blizzard) ||
                     WasLastSpell(Blizzard4)))
                    return LevelChecked(Fire3)
                        ? Fire3
                        : Fire;

                if (curMp + nextMpGain <= 10000 || curMp < 7500)
                    return Blizzard;

                if (IsEnabled(CustomComboPreset.BLM_ST_Transpose) &&
                    ActionReady(Transpose) && canWeave &&
                    curMp is MP.MaxMP && HasEffect(Buffs.Firestarter))
                    return Transpose;

                return LevelChecked(Fire3)
                    ? Fire3
                    : Transpose;
            }

            if (Blizzard3.LevelChecked())
                return Blizzard3;

            return actionID;
        }
    }

    internal class BLM_AoE_SimpleMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AoE_SimpleMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID is not (Blizzard2 or HighBlizzard2))
                return actionID;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.BLM_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave(ActionWatching.LastSpell))
                return Variant.VariantRampart;

            if (WasLastSpell(UmbralSoul))
                return OriginalHook(Fire2);

            if ((HasEffect(Buffs.Thunderhead) && gcdsInTimer > 1 && Thunder2.LevelChecked() &&
                 thunderDebuffAoE is null) || thunderDebuffAoE?.RemainingTime < 3)
                return OriginalHook(Thunder2);

            if (ActionReady(Amplifier) && remainingPolyglotCD >= 20000 && CanSpellWeave(ActionWatching.LastSpell))
                return Amplifier;

            if (IsMoving)
            {
                if (ActionReady(Amplifier) && Gauge.PolyglotStacks < maxPolyglot)
                    return Amplifier;

                if (HasPolyglotStacks(Gauge))
                    return Foul;
            }

            if (CanSpellWeave(ActionWatching.LastSpell) &&
                ActionReady(LeyLines) && !HasEffect(Buffs.LeyLines))
                return LeyLines;

            if (Gauge.InAstralFire)
            {
                if (curMp == 0 && FlareStar.LevelChecked() && Gauge.AstralSoulStacks == 6)
                    return FlareStar;

                if (!FlareStar.LevelChecked() && Fire2.LevelChecked() && curMp >= MP.FireAoE &&
                    (Gauge.UmbralHearts > 1 || !TraitLevelChecked(Traits.UmbralHeart)))
                    return OriginalHook(Fire2);

                if (Flare.LevelChecked() && curMp >= MP.FlareAoE)
                {
                    if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0 &&
                        CanSpellWeave(ActionWatching.LastSpell))
                        return Triplecast;

                    return Flare;
                }

                if (Fire2.LevelChecked())
                    if (gcdsInTimer > 1 && curMp >= MP.FireAoE)
                        return OriginalHook(Fire2);

                if (ActionReady(Manafont))
                    return Manafont;

                if (ActionReady(Transpose) && (!TraitLevelChecked(Traits.AspectMasteryIII) || canSwiftF))
                    return Transpose;

                if (ActionReady(Blizzard2) && TraitLevelChecked(Traits.AspectMasteryIII))
                    return OriginalHook(Blizzard2);
            }

            if (Gauge.InUmbralIce)
            {
                if (HasPolyglotStacks(Gauge))
                    return Foul;

                if (ActionWatching.WhichOfTheseActionsWasLast(OriginalHook(Fire2), OriginalHook(Freeze),
                        OriginalHook(Flare), OriginalHook(FlareStar)) == OriginalHook(Freeze) &&
                    FlareStar.LevelChecked())
                {
                    if (ActionReady(Transpose) && CanSpellWeave(ActionWatching.LastSpell))
                        return Transpose;

                    return OriginalHook(Fire2);
                }

                if (ActionReady(OriginalHook(Blizzard2)) && Gauge.UmbralIceStacks < 3 &&
                    TraitLevelChecked(Traits.AspectMasteryIII))
                {
                    if (ActionReady(Triplecast) && GetBuffStacks(Buffs.Triplecast) == 0 &&
                        CanSpellWeave(ActionWatching.LastSpell))
                        return Triplecast;

                    if (GetBuffStacks(Buffs.Triplecast) == 0 && IsOffCooldown(All.Swiftcast) &&
                        CanSpellWeave(ActionWatching.LastSpell))
                        return All.Swiftcast;

                    if (HasEffect(All.Buffs.Swiftcast) || GetBuffStacks(Buffs.Triplecast) > 0)
                        return OriginalHook(Blizzard2);
                }

                if (Gauge.UmbralIceStacks < 3 && ActionReady(OriginalHook(Blizzard2)))
                    return OriginalHook(Blizzard2);

                if (Freeze.LevelChecked() && Gauge.UmbralHearts < 3 && TraitLevelChecked(Traits.UmbralHeart))
                    return Freeze;

                if (BLMHelper.DoubleBlizz() && Fire2.LevelChecked())
                    return OriginalHook(Fire2);

                if (curMp < LocalPlayer.MaxMp)
                    return Freeze.LevelChecked()
                        ? OriginalHook(Freeze)
                        : OriginalHook(Blizzard2);

                if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                    ActionReady(Transpose) &&
                    ((CanSpellWeave(ActionWatching.LastSpell) && Flare.LevelChecked()) ||
                     !TraitLevelChecked(Traits.AspectMasteryIII)))
                    return Transpose;

                if (Fire2.LevelChecked() && TraitLevelChecked(Traits.AspectMasteryIII))
                    return OriginalHook(Fire2);
            }

            if (Blizzard2.LevelChecked())
                return OriginalHook(Blizzard2);

            return actionID;
        }
    }

    internal class BLM_AoE_AdvancedMode : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_AoE_AdvancedMode;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            int PolyglotStacks = Gauge.PolyglotStacks;
            float TriplecastChargetime = GetCooldownChargeRemainingTime(Triplecast);

            if (actionID is not (Blizzard2 or HighBlizzard2))
                return actionID;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Cure) &&
                IsEnabled(Variant.VariantCure) &&
                PlayerHealthPercentageHp() <= Config.BLM_VariantCure)
                return Variant.VariantCure;

            if (IsEnabled(CustomComboPreset.BLM_Variant_Rampart) &&
                IsEnabled(Variant.VariantRampart) &&
                IsOffCooldown(Variant.VariantRampart) &&
                CanSpellWeave(ActionWatching.LastSpell))
                return Variant.VariantRampart;

            if (WasLastSpell(UmbralSoul))
                return OriginalHook(Fire2);

            if ((IsEnabled(CustomComboPreset.BLM_AoE_Thunder) &&
                 HasEffect(Buffs.Thunderhead) && gcdsInTimer > 1 && LevelChecked(Thunder2) &&
                 GetTargetHPPercent() >= Config.BLM_AoE_ThunderHP &&
                 thunderDebuffAoE is null) || thunderDebuffAoE?.RemainingTime < 3)
                return OriginalHook(Thunder2);

            if (IsEnabled(CustomComboPreset.BLM_AoE_Amplifier) &&
                ActionReady(Amplifier) && remainingPolyglotCD >= 20000 && CanSpellWeave(ActionWatching.LastSpell))
                return Amplifier;

            if (IsMoving)
            {
                if (IsEnabled(CustomComboPreset.BLM_AoE_Amplifier) &&
                    ActionReady(Amplifier) && Gauge.PolyglotStacks < maxPolyglot)
                    return Amplifier;

                if (IsEnabled(CustomComboPreset.BLM_AoE_UsePolyglotMoving) &&
                    PolyglotStacks > Config.BLM_AoE_UsePolyglotMoving_HoldCharges)
                    return Foul;
            }

            if (IsEnabled(CustomComboPreset.BLM_AoE_LeyLines) &&
                CanSpellWeave(ActionWatching.LastSpell) &&
                ActionReady(LeyLines) && !HasEffect(Buffs.LeyLines) &&
                GetRemainingCharges(LeyLines) > Config.BLM_AoE_LeyLinesCharges)
                return LeyLines;

            if (Gauge.InAstralFire)
            {
                if (IsEnabled(CustomComboPreset.BLM_AoE_FlareStar) &&
                    curMp == 0 && FlareStar.LevelChecked() && Gauge.AstralSoulStacks == 6)
                    return FlareStar;

                if (!FlareStar.LevelChecked() && Fire2.LevelChecked() && curMp >= MP.FireAoE &&
                    (Gauge.UmbralHearts > 1 || !TraitLevelChecked(Traits.UmbralHeart)))
                    return OriginalHook(Fire2);

                if (IsEnabled(CustomComboPreset.BLM_AoE_Flare) &&
                    Flare.LevelChecked() && curMp >= MP.FlareAoE)
                {
                    if (LevelChecked(Triplecast) && CanSpellWeave(ActionWatching.LastSpell) &&
                        GetBuffStacks(Buffs.Triplecast) == 0 &&
                        (GetRemainingCharges(Triplecast) > Config.BLM_AoE_Triplecast_HoldCharges ||
                         TriplecastChargetime <= Config.BLM_AoE_Triplecast_ChargeTime))
                        return Triplecast;

                    return Flare;
                }

                if (Fire2.LevelChecked())
                    if (gcdsInTimer > 1 && curMp >= MP.FireAoE)
                        return OriginalHook(Fire2);

                if (IsEnabled(CustomComboPreset.BLM_AoE_Manafont) &&
                    ActionReady(Manafont))
                    return Manafont;

                if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                    ActionReady(Transpose) && (!TraitLevelChecked(Traits.AspectMasteryIII) || canSwiftF))
                    return Transpose;

                if (ActionReady(Blizzard2) && TraitLevelChecked(Traits.AspectMasteryIII))
                    return OriginalHook(Blizzard2);
            }

            if (Gauge.InUmbralIce)
            {
                if (IsEnabled(CustomComboPreset.BLM_AoE_UsePolyglot) &&
                    PolyglotStacks > Config.BLM_AoE_UsePolyglot_HoldCharges)
                    return Foul;

                if (ActionWatching.WhichOfTheseActionsWasLast(OriginalHook(Fire2), OriginalHook(Freeze),
                        OriginalHook(Flare), OriginalHook(FlareStar)) == OriginalHook(Freeze) &&
                    FlareStar.LevelChecked())
                {
                    if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                        ActionReady(Transpose) && CanSpellWeave(ActionWatching.LastSpell))
                        return Transpose;

                    return OriginalHook(Fire2);
                }

                if (ActionReady(OriginalHook(Blizzard2)) && Gauge.UmbralIceStacks < 3 &&
                    TraitLevelChecked(Traits.AspectMasteryIII))
                {
                    if (IsEnabled(CustomComboPreset.BLM_AoE_Triplecast) &&
                        LevelChecked(Triplecast) && CanSpellWeave(ActionWatching.LastSpell) &&
                        GetBuffStacks(Buffs.Triplecast) == 0 &&
                        (GetRemainingCharges(Triplecast) > Config.BLM_AoE_Triplecast_HoldCharges ||
                         TriplecastChargetime <= Config.BLM_AoE_Triplecast_ChargeTime))
                        return Triplecast;

                    if (IsEnabled(CustomComboPreset.BLM_AoE_Swiftcast) &&
                        GetBuffStacks(Buffs.Triplecast) == 0 && IsOffCooldown(All.Swiftcast) &&
                        CanSpellWeave(ActionWatching.LastSpell))
                        return All.Swiftcast;

                    if (HasEffect(All.Buffs.Swiftcast) || GetBuffStacks(Buffs.Triplecast) > 0)
                        return OriginalHook(Blizzard2);
                }

                if (Gauge.UmbralIceStacks < 3 && ActionReady(OriginalHook(Blizzard2)))
                    return OriginalHook(Blizzard2);

                if (Freeze.LevelChecked() && Gauge.UmbralHearts < 3 && TraitLevelChecked(Traits.UmbralHeart))
                    return Freeze;

                if (BLMHelper.DoubleBlizz() && Fire2.LevelChecked())
                    return OriginalHook(Fire2);

                if (curMp < LocalPlayer.MaxMp)
                    return Freeze.LevelChecked()
                        ? OriginalHook(Freeze)
                        : OriginalHook(Blizzard2);

                if (IsEnabled(CustomComboPreset.BLM_AoE_Transpose) &&
                    ActionReady(Transpose) &&
                    ((CanSpellWeave(ActionWatching.LastSpell) && Flare.LevelChecked()) ||
                     !TraitLevelChecked(Traits.AspectMasteryIII)))
                    return Transpose;

                if (Fire2.LevelChecked() && TraitLevelChecked(Traits.AspectMasteryIII))
                    return OriginalHook(Fire2);
            }

            if (Blizzard2.LevelChecked())
                return OriginalHook(Blizzard2);

            return actionID;
        }
    }

    internal class BLM_Variant_Raise : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Variant_Raise;

        protected override uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level) =>
            actionID is All.Swiftcast && HasEffect(All.Buffs.Swiftcast) && IsEnabled(Variant.VariantRaise)
                ? Variant.VariantRaise
                : actionID;
    }

    internal class BLM_Scathe_Xeno : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Scathe_Xeno;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
            actionID is Scathe && LevelChecked(Xenoglossy) && HasPolyglotStacks(Gauge)
                ? Xenoglossy
                : actionID;
    }

    internal class BLM_Blizzard_1to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Blizzard_1to3;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            switch (actionID)
            {
                case Blizzard when LevelChecked(Freeze) && !Gauge.InUmbralIce:
                    return Blizzard3;

                case Freeze when !LevelChecked(Freeze):
                    return Blizzard2;

                default:
                    return actionID;
            }
        }
    }

    internal class BLM_Fire_1to3 : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Fire_1to3;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
            actionID is Fire &&
            ((LevelChecked(Fire3) && !Gauge.InAstralFire) ||
             HasEffect(Buffs.Firestarter))
                ? Fire3
                : actionID;
    }

    internal class BLM_Between_The_LeyLines : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Between_The_LeyLines;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
            actionID is LeyLines && HasEffect(Buffs.LeyLines) && LevelChecked(BetweenTheLines)
                ? BetweenTheLines
                : actionID;
    }

    internal class BLM_Aetherial_Manipulation : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_Aetherial_Manipulation;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
            actionID is AetherialManipulation &&
            ActionReady(BetweenTheLines) &&
            HasEffect(Buffs.LeyLines) &&
            !HasEffect(Buffs.CircleOfPower) &&
            !IsMoving
                ? BetweenTheLines
                : actionID;
    }

    internal class BLM_UmbralSoul : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_UmbralSoul;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
            actionID is Transpose && Gauge.InUmbralIce && LevelChecked(UmbralSoul)
                ? UmbralSoul
                : actionID;
    }

    internal class BLM_TriplecastProtection : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_TriplecastProtection;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level) =>
            actionID is Triplecast && HasEffect(Buffs.Triplecast) && LevelChecked(Triplecast)
                ? OriginalHook(11)
                : actionID;
    }

    internal class BLM_FireandIce : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.BLM_FireandIce;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            switch (actionID)
            {
                case Fire4 when (Gauge.InAstralFire && LevelChecked(Fire4)):
                    return Fire4;

                case Fire4 when (Gauge.InUmbralIce && LevelChecked(Blizzard4)):
                    return Blizzard4;

                default:
                    return actionID;
            }
        }
    }

    #region ID's

    public const byte ClassID = 7;
    public const byte JobID = 25;

    public const uint
        Fire = 141,
        Blizzard = 142,
        Thunder = 144,
        Fire2 = 147,
        Transpose = 149,
        Fire3 = 152,
        Thunder3 = 153,
        Blizzard3 = 154,
        AetherialManipulation = 155,
        Scathe = 156,
        Manafont = 158,
        Freeze = 159,
        Flare = 162,
        LeyLines = 3573,
        Blizzard4 = 3576,
        Fire4 = 3577,
        BetweenTheLines = 7419,
        Thunder4 = 7420,
        Triplecast = 7421,
        Foul = 7422,
        Thunder2 = 7447,
        Despair = 16505,
        UmbralSoul = 16506,
        Xenoglossy = 16507,
        Blizzard2 = 25793,
        HighFire2 = 25794,
        HighBlizzard2 = 25795,
        Amplifier = 25796,
        Paradox = 25797,
        HighThunder = 36986,
        HighThunder2 = 36987,
        FlareStar = 36989;

    // Debuff Pairs of Actions and Debuff
    public static readonly Dictionary<uint, ushort>
        ThunderList = new()
        {
            { Thunder, Debuffs.Thunder },
            { Thunder2, Debuffs.Thunder2 },
            { Thunder3, Debuffs.Thunder3 },
            { Thunder4, Debuffs.Thunder4 },
            { HighThunder, Debuffs.HighThunder },
            { HighThunder2, Debuffs.HighThunder2 }
        };

    private static int nextMpGain => Gauge.UmbralIceStacks switch
    {
        0 => 0,
        1 => 2500,
        2 => 5000,
        3 => 10000,
        var _ => 0
    };

    public static class Buffs
    {
        public const ushort
            Thundercloud = 164,
            Firestarter = 165,
            LeyLines = 737,
            CircleOfPower = 738,
            Sharpcast = 867,
            Triplecast = 1211,
            Thunderhead = 3870;
    }

    public static class Debuffs
    {
        public const ushort
            Thunder = 161,
            Thunder2 = 162,
            Thunder3 = 163,
            Thunder4 = 1210,
            HighThunder = 3871,
            HighThunder2 = 3872;
    }

    public static class Traits
    {
        public const uint
            UmbralHeart = 295,
            EnhancedPolyglot = 297,
            AspectMasteryIII = 459,
            EnhancedFoul = 461,
            EnhancedManafont = 463,
            Enochian = 460,
            EnhancedPolyglotII = 615;
    }

    public static class MP
    {
        public const int MaxMP = 10000;

        public const int AllMPSpells = 800; //"ALL MP" spell. Only caring about the absolute minimum.

        public static int FireI => GetResourceCost(OriginalHook(Fire));

        public static int FlareAoE => GetResourceCost(OriginalHook(Flare));

        public static int FireAoE => GetResourceCost(OriginalHook(Fire2));

        public static int FireIII => GetResourceCost(OriginalHook(Fire3));

        public static int BlizzardAoE => GetResourceCost(OriginalHook(Blizzard2));

        public static int BlizzardI => GetResourceCost(OriginalHook(Blizzard));

        public static int Freeze => GetResourceCost(OriginalHook(BLM.Freeze));

        public static int Despair => GetResourceCost(OriginalHook(BLM.Despair));
    }

    #endregion
}