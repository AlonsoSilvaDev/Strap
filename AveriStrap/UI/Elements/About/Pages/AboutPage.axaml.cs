// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Animation;
using AveriStrap.UI.ViewModels.About;

namespace AveriStrap.UI.Elements.About.Pages
{
    internal sealed partial class AboutPage : UserControl
    {
        public AboutPage()
        {
            DataContext = new AboutViewModel();
            InitializeComponent();
        }
    }
}
