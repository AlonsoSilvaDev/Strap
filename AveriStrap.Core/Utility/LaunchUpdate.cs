// SPDX-FileCopyrightText: 2026 AveriStrap
//
// SPDX-License-Identifier: MPL-2.0

namespace AveriStrap.Utility
{
    internal enum LaunchUpdateResult
    {
        Skipped,
        UpToDate,
        Failed,
        Restarting,
    }

    internal sealed record LaunchUpdateRequest
    {
        public required UpdateCheck Setting { get; init; }
        public required string[] Args { get; init; }
        public bool IsCiBuild { get; init; }
        public bool IsBackgroundProcess { get; init; }
        public bool OtherInstancesRunning { get; init; }
        public string? LastAttemptedVersion { get; init; }
        public TimeSpan CheckTimeout { get; init; } = TimeSpan.FromSeconds(8);
        public TimeSpan DownloadTimeout { get; init; } = TimeSpan.FromMinutes(3);
    }

    internal interface ILaunchUpdateSource
    {
        bool IsInstalled { get; }

        /// <summary>Versión nueva disponible, o null si ya está al día.</summary>
        Task<string?> FindUpdateAsync();

        Task DownloadAsync(CancellationToken cancellationToken);

        void RecordAttempt(string version);

        void ApplyAndRestart(string[] args);
    }

    /// <summary>
    /// Actualiza AveriStrap justo antes de abrir Roblox, para quien nunca abre la aplicación por separado.
    /// Ningún fallo impide jugar: solo la cancelación del usuario detiene el arranque.
    /// </summary>
    internal static class LaunchUpdate
    {
        public static async Task<LaunchUpdateResult> RunAsync(
            LaunchUpdateRequest request,
            ILaunchUpdateSource source,
            CancellationToken cancellationToken,
            Action<string>? onFound = null,
            Action<Exception>? onError = null)
        {
            // con otra copia abierta (vigilando otra partida) instalar la cerraría
            if (request.Setting == UpdateCheck.Disabled
                || request.IsCiBuild
                || request.IsBackgroundProcess
                || request.OtherInstancesRunning)
                return LaunchUpdateResult.Skipped;

            try
            {
                if (!source.IsInstalled)
                    return LaunchUpdateResult.Skipped;

                string? version = await source.FindUpdateAsync().WaitAsync(request.CheckTimeout, cancellationToken);

                if (version is null)
                    return LaunchUpdateResult.UpToDate;

                // si ya se intentó y seguimos en la versión vieja, reintentar al jugar sería un bucle
                if (version == request.LastAttemptedVersion)
                    return LaunchUpdateResult.Skipped;

                onFound?.Invoke(version);

                using var download = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                download.CancelAfter(request.DownloadTimeout);

                await source.DownloadAsync(download.Token).WaitAsync(download.Token);

                cancellationToken.ThrowIfCancellationRequested();

                source.RecordAttempt(version);
                source.ApplyAndRestart(request.Args);

                return LaunchUpdateResult.Restarting;
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                onError?.Invoke(ex);
                return LaunchUpdateResult.Failed;
            }
        }
    }
}
