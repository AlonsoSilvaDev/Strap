// SPDX-FileCopyrightText: 2026 AveriStrap
//
// SPDX-License-Identifier: MPL-2.0

namespace AveriStrap.Cli;

/// <summary>
/// Traduce los argumentos de AveriStrap 1.x (un guion), que siguen llegando desde
/// accesos directos, enlaces registrados y la actualización automática antigua.
/// </summary>
internal static class LegacyArguments
{
    private static readonly Dictionary<string, string?> Map = new(StringComparer.OrdinalIgnoreCase)
    {
        ["-player"] = "--player",
        ["-studio"] = "--studio",
        ["-settings"] = "--settings",
        ["-upgrade"] = "--upgrade",
        // sin equivalente: se descartan para que no impidan abrir la app
        ["-menu"] = null,
        ["-quiet"] = null,
    };

    public static string[] Translate(string[] args)
    {
        var result = new List<string>(args.Length);

        foreach (string arg in args)
        {
            if (!Map.TryGetValue(arg, out string? replacement))
                result.Add(arg);
            else if (replacement is not null)
                result.Add(replacement);
        }

        return [.. result];
    }
}
