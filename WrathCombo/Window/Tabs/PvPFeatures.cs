﻿using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ECommons.ImGuiMethods;
using ImGuiNET;
using System.Linq;
using System.Numerics;
using WrathCombo.Core;
using WrathCombo.Services;
using WrathCombo.Window.Functions;

namespace WrathCombo.Window.Tabs
{
    internal class PvPFeatures : ConfigWindow
    {
        internal static string OpenJob = string.Empty;

        internal static new void Draw()
        {
            PvEFeatures.HasToOpenJob = true;

            using (var scrolling = ImRaii.Child("scrolling", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y), true))
            {
                int i = 1;

                var indentwidth = 12f.Scale();
                var indentwidth2 = indentwidth + 42f.Scale();

                if (OpenJob == string.Empty)
                {
                    ImGuiEx.LineCentered("pvpDesc", () =>
                    {
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGui.TextWrapped($"{FontAwesomeIcon.SkullCrossbones.ToIconString()}");
                        ImGui.PopFont();
                        ImGui.SameLine();
                        ImGui.TextWrapped("These are PvP features. They will only work in PvP-enabled zones.");
                        ImGui.SameLine();
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGui.TextWrapped($"{FontAwesomeIcon.SkullCrossbones.ToIconString()}");
                        ImGui.PopFont();
                    });
                    ImGuiEx.LineCentered($"pvpDesc2", () =>
                    {
                        ImGuiEx.TextUnderlined("Select a job from below to enable and configure features for it.");
                    });
                    ImGui.Spacing();

                    foreach (string? jobName in groupedPresets.Where(x => x.Value.Any(y => PresetStorage.IsPvP(y.Preset))).Select(x => x.Key))
                    {
                        string abbreviation = groupedPresets[jobName].First().Info.JobShorthand;
                        string header = string.IsNullOrEmpty(abbreviation) ? jobName : $"{jobName} - {abbreviation}";
                        var id = groupedPresets[jobName].First().Info.JobID;
                        IDalamudTextureWrap? icon = Icons.GetJobIcon(id);
                        using (var disabled = ImRaii.Disabled(DisabledJobsPVP.Any(x => x == id)))
                        {
                            if (ImGui.Selectable($"###{header}", OpenJob == jobName, ImGuiSelectableFlags.None,
                                    icon == null ? new Vector2(0, 32f.Scale()) : new Vector2(0, (icon.Size.Y / 2f).Scale())))
                            {
                                OpenJob = jobName;
                            }
                            ImGui.SameLine(indentwidth);
                            if (icon != null)
                            {
                                ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Size.X.Scale(), icon.Size.Y.Scale()) / 2f);
                                ImGui.SameLine(indentwidth2);
                            }
                            ImGui.Text($"{header} {(disabled ? "(Disabled due to update)" : "")}");

                            if (!string.IsNullOrEmpty(abbreviation) &&
                                P.IPC.UIHelper.JobControlled(id) is not null)
                            {
                                ImGui.SameLine();
                                P.IPC.UIHelper
                                    .ShowIPCControlledIndicatorIfNeeded(id);
                            }
                        }
                    }
                }
                else
                {
                    var id = groupedPresets[OpenJob].First().Info.JobID;
                    IDalamudTextureWrap? icon = Icons.GetJobIcon(id);

                    using (var headingTab = ImRaii.Child("PvPHeadingTab", new Vector2(ImGui.GetContentRegionAvail().X, icon is null ? 24f.Scale() : (icon.Size.Y / 2f.Scale()) + 4f)))
                    {
                        if (ImGui.Button("Back", new Vector2(0, 24f.Scale())))
                        {
                            OpenJob = "";
                            return;
                        }
                        ImGui.SameLine();
                        ImGuiEx.LineCentered(() =>
                        {
                            if (icon != null)
                            {
                                ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Size.X.Scale(), icon.Size.Y.Scale()) / 2f);
                                ImGui.SameLine();
                            }
                            ImGuiEx.Text($"{OpenJob}");
                        });

                    }

                    using (var contents = ImRaii.Child("Contents", new Vector2(0)))
                    {

                        try
                        {
                            if (ImGui.BeginTabBar($"subTab{OpenJob}", ImGuiTabBarFlags.Reorderable | ImGuiTabBarFlags.AutoSelectNewTabs))
                            {
                                if (ImGui.BeginTabItem("Normal"))
                                {
                                    DrawHeadingContents(OpenJob, i);
                                    ImGui.EndTabItem();
                                }

                                ImGui.EndTabBar();
                            }
                        }
                        catch { }

                    }
                }

            }
        }

        private static void DrawHeadingContents(string jobName, int i)
        {
            foreach (var (preset, info) in groupedPresets[jobName].Where(x => PresetStorage.IsPvP(x.Preset)))
            {
                InfoBox presetBox = new() { Color = Colors.Grey, BorderThickness = 1f, CurveRadius = 8f, ContentsAction = () => { Presets.DrawPreset(preset, info, ref i); } };

                if (Service.Configuration.HideConflictedCombos)
                {
                    var conflictOriginals = PresetStorage.GetConflicts(preset); // Presets that are contained within a ConflictedAttribute
                    var conflictsSource = PresetStorage.GetAllConflicts();      // Presets with the ConflictedAttribute

                    if (!conflictsSource.Where(x => x == preset).Any() || conflictOriginals.Length == 0)
                    {
                        presetBox.Draw();
                        ImGuiHelpers.ScaledDummy(12.0f);
                        continue;
                    }

                    if (conflictOriginals.Any(x => PresetStorage.IsEnabled(x)))
                    {
                        Service.Configuration.EnabledActions.Remove(preset);
                        Service.Configuration.Save();

                        // Keep removed items in the counter
                        var parent = PresetStorage.GetParent(preset) ?? preset;
                        i += 1 + Presets.AllChildren(presetChildren[parent]);
                    }

                    else
                    {
                        presetBox.Draw();
                        ImGuiHelpers.ScaledDummy(12.0f);
                        continue;
                    }
                }

                else
                {
                    presetBox.Draw();
                    ImGuiHelpers.ScaledDummy(12.0f);
                }
            }
        }
    }
}
