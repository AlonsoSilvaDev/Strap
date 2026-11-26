// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using AveriStrap.UI.ViewModels.Settings;

namespace AveriStrap.UI.Elements.Settings.Pages
{
    internal sealed partial class RegionSelectorPage : UserControl
    {
        public RegionSelectorPage()
        {
            InitializeComponent();

            AttachedToVisualTree += (_, _) => { if (!MainWindow.IsStagingPages) App.AveriRPC?.SetPage("Region Selector"); };

            GameSearchAutoCompleteBox.SelectionChanged += OnGameSearchSelectionChanged;
        }

        private void OnGameSearchSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (DataContext is RegionSelectorViewModel vm &&
                e.AddedItems.Count > 0 &&
                e.AddedItems[0] is OmniSearchContent selected)
            {
                vm.PlaceId = selected.RootPlaceId.ToString(CultureInfo.InvariantCulture);
                vm.SearchQuery = selected.RootPlaceId.ToString(CultureInfo.InvariantCulture);
                vm.IsSearchFlyoutOpen = false;
                if (vm.SearchCommand.CanExecute(null))
                    _ = vm.SearchCommand.ExecuteAsync(null);
            }
        }
    }
}