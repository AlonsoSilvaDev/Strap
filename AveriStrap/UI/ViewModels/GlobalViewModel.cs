// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace AveriStrap.UI.ViewModels;

internal static class GlobalViewModel
{
    public static bool IsWindows => OperatingSystem.IsWindows();
    public static bool IsMacOS => OperatingSystem.IsMacOS();
    public static bool IsLinux => OperatingSystem.IsLinux();

    public static bool IsWindowsOrLinux => IsWindows || IsLinux;
    public static bool IsWindowsOrMacOS => IsWindows || IsMacOS;

    public static bool HasClientSettingsPage => App.Clients.Player.SettingsSync is not null || App.Clients.Studio.RuntimeEnvironment is not null;

    public static ICommand OpenWebpageCommand => new RelayCommand<string>(OpenWebpage);

    // AveriStrap todavía no tiene servidor de Discord: se avisa en vez de abrir uno
    public static ICommand OpenDiscordCommand => new AsyncRelayCommand(
        () => Frontend.ShowMessageBox(Strings.Dialog_DiscordNotCreated, MessageBoxImage.Information));

    private static void OpenWebpage(string? location)
    {
        if (string.IsNullOrEmpty(location))
            return;

        AveriStrap.Utility.Threading.ShellExecute(location);
    }
}
