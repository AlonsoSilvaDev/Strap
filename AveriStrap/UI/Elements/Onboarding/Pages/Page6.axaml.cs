// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

using Avalonia.Controls;
using AveriStrap.UI.ViewModels.Onboarding;

namespace AveriStrap.UI.Elements.Onboarding.Pages
{
    internal sealed partial class Page6 : UserControl
    {
        public Page6()
        {
            DataContext = new Page6ViewModel();
            InitializeComponent();
        }
    }
}
