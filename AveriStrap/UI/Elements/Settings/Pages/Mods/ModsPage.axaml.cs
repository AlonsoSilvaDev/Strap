// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using AveriStrap.UI.ViewModels.Settings.Mods;

namespace AveriStrap.UI.Elements.Settings.Pages.Mods
{
    internal sealed partial class ModsPage : UserControl
    {
        public ModsPage()
        {
            InitializeComponent();
            AttachedToVisualTree += (_, _) => { if (!MainWindow.IsStagingPages) App.AveriRPC?.SetPage("Mods"); };
        }

        private void Page_DragEnter(object? sender, DragEventArgs e)
        {
            e.Handled = true;
            if (DataContext is ModsViewModel vm)
                vm.IsDragOver = true;
        }

        private void Page_DragLeave(object? sender, DragEventArgs e)
        {
            e.Handled = true;
            if (DataContext is ModsViewModel vm)
                vm.IsDragOver = false;
        }

        private async void Page_Drop(object? sender, DragEventArgs e)
        {
            e.Handled = true;
            if (DataContext is ModsViewModel vm)
            {
                vm.IsDragOver = false;

                var files = e.DataTransfer.TryGetFiles();
                if (files != null)
                {
                    var paths = files
                        .Select(f => f.TryGetLocalPath())
                        .Where(p => !string.IsNullOrEmpty(p))
                        .ToArray();

                    if (paths.Length > 0)
                        await vm.ImportFromPaths(paths!);
                }
            }
        }
    }
}