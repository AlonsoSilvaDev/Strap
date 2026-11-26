// SPDX-FileCopyrightText: 2026 Froststrap
//
// SPDX-License-Identifier: MPL-2.0

namespace AveriStrap.Clients.Contracts;

internal interface IPackageReporter : IInstallReporter
{
    void AddDownloaded(long bytes, bool updateProgressBar = true);

    void AddTotal(long bytes);

    void MarkExtractionFailed();

    Task OnChecksumFailedAsync(Exception exception);
}
