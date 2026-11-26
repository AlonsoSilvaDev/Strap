// SPDX-FileCopyrightText: 2026 AveriStrap
//
// SPDX-License-Identifier: MPL-2.0

namespace AveriStrap.Utility
{
    internal enum LaunchRoute
    {
        Onboarding,
        Settings,
        Roblox,
        Menu,
    }

    internal static class LaunchRouting
    {
        public static LaunchRoute Decide(bool isFirstLaunch, bool settingsRequested, LaunchMode robloxMode)
        {
            // quien viene a jugar entra al juego; la bienvenida espera a que abra la aplicación
            if (!settingsRequested && robloxMode != LaunchMode.None)
                return LaunchRoute.Roblox;

            if (isFirstLaunch)
                return LaunchRoute.Onboarding;

            return settingsRequested ? LaunchRoute.Settings : LaunchRoute.Menu;
        }
    }
}
