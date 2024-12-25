﻿using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using System.Numerics;
using WrathCombo.CustomComboNS.Functions;
using WrathCombo.Services;

namespace WrathCombo.Window.Tabs
{
    internal class Settings : ConfigWindow
    {
        internal static new void Draw()
        {
            PvEFeatures.HasToOpenJob = true;
            using (var child = ImRaii.Child("main", new Vector2(0, 0), true))
            {
                ImGui.Text("This tab allows you to customise your options when enabling features.");

                #region SubCombos

                bool hideChildren = Service.Configuration.HideChildren;

                if (ImGui.Checkbox("Hide SubCombo Options", ref hideChildren))
                {
                    Service.Configuration.HideChildren = hideChildren;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Hides the sub-options of disabled features.");
                    ImGui.EndTooltip();
                }
                ImGui.NextColumn();

                #endregion

                #region Conflicting

                bool hideConflicting = Service.Configuration.HideConflictedCombos;
                if (ImGui.Checkbox("Hide Conflicted Combos", ref hideConflicting))
                {
                    Service.Configuration.HideConflictedCombos = hideConflicting;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Hides any combos that conflict with others you have selected.");
                    ImGui.EndTooltip();
                }

                #endregion

                #region Combat Log

                bool showCombatLog = Service.Configuration.EnabledOutputLog;

                if (ImGui.Checkbox("Output Log to Chat", ref showCombatLog))
                {
                    Service.Configuration.EnabledOutputLog = showCombatLog;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Every time you use an action, the plugin will print it to the chat.");
                    ImGui.EndTooltip();
                }
                #endregion

                #region Melee Offset
                float offset = (float)Service.Configuration.MeleeOffset;
                ImGui.PushItemWidth(75);

                bool inputChangedeth = false;
                inputChangedeth |= ImGui.InputFloat("Melee Distance Offset", ref offset);

                if (inputChangedeth)
                {
                    Service.Configuration.MeleeOffset = (double)offset;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Offset of melee check distance for features that use it.\r\nFor those who don't want to immediately use their ranged attack if the boss walks slightly out of range.");
                    ImGui.EndTooltip();
                }
                #endregion

                #region Message of the Day

                bool motd = Service.Configuration.HideMessageOfTheDay;

                if (ImGui.Checkbox("Hide Message of the Day", ref motd))
                {
                    Service.Configuration.HideMessageOfTheDay = motd;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted("Disables the Message of the Day message in your chat when you login.");
                    ImGui.EndTooltip();
                }
                ImGui.NextColumn();

                #endregion

                #region TargetHelper

                Vector4 colour = Service.Configuration.TargetHighlightColor;
                if (ImGui.ColorEdit4("Target Highlight Colour", ref colour, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar))
                {
                    Service.Configuration.TargetHighlightColor = colour;
                    Service.Configuration.Save();
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.TextUnformatted($"Used for {CustomComboFunctions.JobIDs.JobIDToName(33)} card targeting features.\r\nSet Alpha to 0 to hide the box.");
                    ImGui.EndTooltip();
                }

                #endregion

                if (ImGui.Checkbox($"Block spells if moving", ref Service.Configuration.BlockSpellOnMove))
                    Service.Configuration.Save();

                if (ImGui.Checkbox($"Output opener status to chat", ref Service.Configuration.OutputOpenerLogs))
                    Service.Configuration.Save();

                if (ImGui.InputFloat("Movement check delay", ref Service.Configuration.MovementLeeway))
                    Service.Configuration.Save();

                ImGuiComponents.HelpMarker("Many features check if you are moving to decide actions. This will allow you to set a delay on how long you need to be moving before it recognizes you as moving.");
            }
        }
    }
}
