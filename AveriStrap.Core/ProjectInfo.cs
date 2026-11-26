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

        public const string BundleId = "xyz.averistrap.desktop";

        public const string RegistryKey = $@"Software\{Name}";
    }
}
