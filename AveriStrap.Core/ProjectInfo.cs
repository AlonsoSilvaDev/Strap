// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

namespace AveriStrap
{
    internal static class ProjectInfo
    {
        public const string Name = "AveriStrap";

        public static bool IsTestBuild => false;

        public const string Repository = "AlonsoSilvaDev/Strap";

        public const string RemoteDataUrl = "https://raw.githubusercontent.com/Froststrap/config/refs/heads/main/Data.json";

        // aplicación de Discord de AveriStrap (discord.com/developers). Vacía = no se muestra
        // "usando AveriStrap" en Discord ni su opción en Integraciones.
        public const string DiscordAppId = "";

        public static bool HasDiscordApp => DiscordAppId.Length > 0;

        public const string BundleId = "xyz.averistrap.desktop";

        public const string RegistryKey = $@"Software\{Name}";
    }
}
