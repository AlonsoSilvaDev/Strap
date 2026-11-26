// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

using AveriStrap.UI.ViewModels.Dialogs;

namespace AveriStrap.UI.Elements.Dialogs
{
    internal sealed partial class ManualCookieDialog : Base.AvaloniaWindow
    {
        public ManualCookieDialogViewModel ViewModel { get; }

        public ManualCookieDialog()
        {
            ViewModel = new ManualCookieDialogViewModel(this);
            DataContext = ViewModel;

            InitializeComponent();
        }
    }
}