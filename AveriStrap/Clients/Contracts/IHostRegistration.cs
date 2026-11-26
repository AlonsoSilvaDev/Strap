// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

namespace AveriStrap.Clients.Contracts;

internal interface IHostRegistration
{
    void Register(string? versionDirectory);

    void ClearLocation();

    void RemoveStartupEntry();

    bool RequestsAdmin(string executablePath);

    void MigrateCompatibilityFlags(string oldExecutablePath, string newExecutablePath);
}
