// SPDX-FileCopyrightText: 2026 AveriStrap
//
// SPDX-License-Identifier: MPL-2.0

using Velopack;

namespace AveriStrap.Startup;

/// <summary>Conecta <see cref="LaunchUpdate"/> con Velopack.</summary>
internal sealed class VelopackLaunchUpdateSource(UpdateCheck setting) : ILaunchUpdateSource
{
    private readonly UpdaterManager _updater = new(setting is UpdateCheck.Test or UpdateCheck.Both);
    private UpdateInfo? _update;

    public bool IsInstalled => _updater.IsInstalled;

    public async Task<string?> FindUpdateAsync()
    {
        _update = await _updater.CheckForUpdatesAsync();
        return _update?.TargetFullRelease.Version.ToString();
    }

    public Task DownloadAsync(CancellationToken cancellationToken)
        => _updater.DownloadUpdatesAsync(_update!, cancellationToken: cancellationToken);

    public void RecordAttempt(string version)
    {
        App.State.Prop.LaunchUpdateAttempt = version;
        App.State.Save();
    }

    public void ApplyAndRestart(string[] args) => _updater.ApplyUpdatesAndRestart(_update!, args);
}
