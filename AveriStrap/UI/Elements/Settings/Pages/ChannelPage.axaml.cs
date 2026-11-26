// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

using Avalonia.Controls;

namespace AveriStrap.UI.Elements.Settings.Pages;

internal sealed partial class ChannelPage : UserControl
{
    public ChannelPage()
    {
        InitializeComponent();

        AttachedToVisualTree += (_, _) => { if (!MainWindow.IsStagingPages) App.AveriRPC?.SetPage("Deployment"); };
    }
}