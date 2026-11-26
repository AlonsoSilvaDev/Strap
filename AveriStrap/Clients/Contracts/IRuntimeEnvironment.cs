// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

namespace AveriStrap.Clients.Contracts;

internal interface IRuntimeEnvironment
{
    Task PrepareAsync(string versionDirectory, string clientSettingsRoot, IInstallReporter reporter, CancellationToken cancellationToken);
}
