// SPDX-FileCopyrightText: 2026 AveriStrap
//
// SPDX-License-Identifier: MPL-2.0

namespace AveriStrap.Utility
{
    internal static class Branding
    {
        // los datos remotos son de un tercero: no se muestra lo que lleve su marca
        public static bool IsForeignBrand(string? name)
            => name?.Contains("Froststrap", StringComparison.OrdinalIgnoreCase) == true;
    }
}
