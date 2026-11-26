// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

using AveriStrap.Integrations;
using AveriStrap.UI.ViewModels.ContextMenu;

namespace AveriStrap.UI.Elements.ContextMenu
{
    /// <summary>
    /// Interaction logic for ServerHistory.axaml
    /// </summary>
    internal sealed partial class ServerHistory : Base.AvaloniaWindow
    {
        public ServerHistory()
        {
            InitializeComponent();
        }

        public ServerHistory(ActivityWatcher watcher) : this()
        {
            var viewModel = new ServerHistoryViewModel(watcher);

            viewModel.RequestCloseEvent += (_, _) => Close();

            DataContext = viewModel;
        }
    }
}