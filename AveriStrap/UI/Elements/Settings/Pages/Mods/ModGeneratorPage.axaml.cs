// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

using Avalonia.Controls;

namespace AveriStrap.UI.Elements.Settings.Pages.Mods
{
    internal sealed partial class ModGeneratorPage : UserControl
    {
        public ModGeneratorPage()
        {
            InitializeComponent();
            AttachedToVisualTree += (_, _) => { if (!MainWindow.IsStagingPages) App.AveriRPC?.SetPage("Mod Generator"); };
        }
    }
}
