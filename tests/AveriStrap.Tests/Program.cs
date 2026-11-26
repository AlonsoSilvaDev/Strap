// SPDX-FileCopyrightText: 2026 AveriStrap
//
// SPDX-License-Identifier: MPL-2.0

using AveriStrap.Enums;
using AveriStrap.Utility;

int failures = 0;

async Task Test(string name, Func<Task> body)
{
    try
    {
        await body();
        Console.WriteLine($"OK     {name}");
    }
    catch (Exception ex)
    {
        failures++;
        Console.WriteLine($"FALLA  {name}: {ex.Message}");
    }
}

static void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new Exception($"se esperaba '{expected}' y salió '{actual}'");
}

static LaunchUpdateRequest Request() => new()
{
    Setting = UpdateCheck.Stable,
    Args = ["--player", "roblox-player:1+launchmode:play"],
    CheckTimeout = TimeSpan.FromMilliseconds(200),
    DownloadTimeout = TimeSpan.FromMilliseconds(200),
};

static Task<LaunchUpdateResult> Run(LaunchUpdateRequest request, FakeSource source, CancellationToken token = default)
    => LaunchUpdate.RunAsync(request, source, token);

await Test("versión nueva: descarga, anota el intento y reinicia con los mismos argumentos", async () =>
{
    var source = new FakeSource { Latest = "1.1.6" };

    Equal(LaunchUpdateResult.Restarting, await Run(Request(), source));
    Equal("check,download,record 1.1.6,apply", string.Join(",", source.Calls));
    Equal("--player|roblox-player:1+launchmode:play", string.Join("|", source.AppliedArgs!));
});

await Test("sin versión nueva: no descarga", async () =>
{
    var source = new FakeSource();

    Equal(LaunchUpdateResult.UpToDate, await Run(Request(), source));
    Equal("check", string.Join(",", source.Calls));
});

foreach (var (name, request, installed) in new (string, LaunchUpdateRequest, bool)[]
{
    ("actualizaciones desactivadas", Request() with { Setting = UpdateCheck.Disabled }, true),
    ("compilación de CI", Request() with { IsCiBuild = true }, true),
    ("actualizador de Roblox en segundo plano", Request() with { IsBackgroundProcess = true }, true),
    ("otra copia abierta", Request() with { OtherInstancesRunning = true }, true),
    ("copia no instalada", Request(), false),
})
{
    await Test($"no comprueba nada: {name}", async () =>
    {
        var source = new FakeSource { Latest = "1.1.6", IsInstalled = installed };

        Equal(LaunchUpdateResult.Skipped, await Run(request, source));
        Equal("", string.Join(",", source.Calls));
    });
}

await Test("si esa versión ya se intentó instalar, no entra en bucle", async () =>
{
    var source = new FakeSource { Latest = "1.1.6" };

    Equal(LaunchUpdateResult.Skipped, await Run(Request() with { LastAttemptedVersion = "1.1.6" }, source));
    Equal("check", string.Join(",", source.Calls));
});

await Test("un intento anterior de otra versión no bloquea la siguiente", async () =>
{
    var source = new FakeSource { Latest = "1.1.7" };

    Equal(LaunchUpdateResult.Restarting, await Run(Request() with { LastAttemptedVersion = "1.1.6" }, source));
});

await Test("sin conexión: se sigue al juego", async () =>
{
    var source = new FakeSource { OnCheck = () => throw new HttpRequestException("sin red") };

    Equal(LaunchUpdateResult.Failed, await Run(Request(), source));
});

await Test("comprobación colgada: vence el plazo y se sigue al juego", async () =>
{
    var source = new FakeSource { OnCheck = () => new TaskCompletionSource().Task };

    Equal(LaunchUpdateResult.Failed, await Run(Request(), source).WaitAsync(TimeSpan.FromSeconds(5)));
});

await Test("descarga fallida: no instala ni anota el intento", async () =>
{
    var source = new FakeSource { Latest = "1.1.6", OnDownload = _ => throw new IOException("disco lleno") };

    Equal(LaunchUpdateResult.Failed, await Run(Request(), source));
    Equal("check,download", string.Join(",", source.Calls));
});

await Test("descarga colgada: vence el plazo y se sigue al juego", async () =>
{
    var source = new FakeSource { Latest = "1.1.6", OnDownload = token => Task.Delay(Timeout.Infinite, token) };

    Equal(LaunchUpdateResult.Failed, await Run(Request(), source).WaitAsync(TimeSpan.FromSeconds(5)));
});

await Test("fallo al instalar: se sigue al juego", async () =>
{
    var source = new FakeSource { Latest = "1.1.6", OnApply = () => throw new InvalidOperationException("ocupado") };

    Equal(LaunchUpdateResult.Failed, await Run(Request(), source));
});

await Test("si el usuario cancela, se cancela el arranque", async () =>
{
    using var cancel = new CancellationTokenSource();
    var source = new FakeSource
    {
        Latest = "1.1.6",
        OnDownload = token => { cancel.Cancel(); return Task.Delay(Timeout.Infinite, token); },
    };

    try
    {
        await Run(Request(), source, cancel.Token);
        throw new Exception("no se canceló");
    }
    catch (OperationCanceledException) { }

    Equal("check,download", string.Join(",", source.Calls));
});

// qué se abre al arrancar: (primera vez, --settings, modo de Roblox) -> destino
foreach (var (name, firstLaunch, settings, mode, expected) in new[]
{
    ("recién actualizado desde la 1.1.4 y entra a jugar", true, false, LaunchMode.Player, LaunchRoute.Roblox),
    ("recién actualizado y abre Studio", true, false, LaunchMode.Studio, LaunchRoute.Roblox),
    ("inicio de sesión de Studio la primera vez", true, false, LaunchMode.StudioAuth, LaunchRoute.Roblox),
    ("primera vez abriendo la aplicación", true, false, LaunchMode.None, LaunchRoute.Onboarding),
    ("primera vez pidiendo ajustes", true, true, LaunchMode.None, LaunchRoute.Onboarding),
    ("ajustes", false, true, LaunchMode.None, LaunchRoute.Settings),
    ("ajustes tiene prioridad sobre jugar", false, true, LaunchMode.Player, LaunchRoute.Settings),
    ("jugar", false, false, LaunchMode.Player, LaunchRoute.Roblox),
    ("menú", false, false, LaunchMode.None, LaunchRoute.Menu),
})
{
    await Test($"arranque: {name}", () =>
    {
        Equal(expected, LaunchRouting.Decide(firstLaunch, settings, mode));
        return Task.CompletedTask;
    });
}

// marca: nada visible para el usuario puede venir del proyecto original
string raiz = AppContext.BaseDirectory;
while (!File.Exists(Path.Combine(raiz, "AveriStrap.slnx")))
    raiz = Path.GetDirectoryName(raiz) ?? throw new Exception("no se encontró la raíz del proyecto");

var fuentes = new[] { "AveriStrap", "AveriStrap.Core", "AveriStrap.Localisation" }
    .SelectMany(d => Directory.EnumerateFiles(Path.Combine(raiz, d), "*", SearchOption.AllDirectories))
    .Where(f => Path.GetExtension(f) is ".cs" or ".axaml" or ".json" or ".xml" or ".csproj")
    .Where(f => !f.Split(Path.DirectorySeparatorChar).Any(p => p is "bin" or "obj"))
    .ToList();

IEnumerable<string> Buscar(string texto) => fuentes.SelectMany(f => File.ReadLines(f)
    .Select((linea, i) => (linea, i))
    .Where(x => x.linea.Contains(texto, StringComparison.OrdinalIgnoreCase))
    .Select(x => $"{Path.GetRelativePath(raiz, f)}:{x.i + 1}: {x.linea.Trim()}"));

// lo que no se puede renombrar: avisos de autoría (MPL), dependencias y direcciones de terceros
string[] permitido =
[
    "SPDX-FileCopyrightText",
    "Include=\"Froststrap.",
    "raw.githubusercontent.com/Froststrap/config",
    "api.github.com/repos/Froststrap/FroststrapStudioRPC",
    "FrostBorderDefault",
    "Branding.cs:",
];

await Test("marca: el código no menciona Froststrap fuera de lo obligatorio", () =>
{
    var restos = Buscar("Froststrap").Where(l => !permitido.Any(p => l.Contains(p))).ToList();
    Equal("", string.Join(" | ", restos.Take(5)));
    return Task.CompletedTask;
});

await Test("marca: no se descargan traducciones ajenas que pisen los textos", () =>
{
    Equal("", string.Join(" | ", Buscar("crowdin.net")));
    return Task.CompletedTask;
});

await Test("marca: no se usa la aplicación de Discord de Froststrap", () =>
{
    Equal("", string.Join(" | ", Buscar("1399535282713399418")));
    return Task.CompletedTask;
});

await Test("marca: no hay invitaciones a servidores de Discord ajenos", () =>
{
    Equal("", string.Join(" | ", Buscar("discord.gg/")));
    return Task.CompletedTask;
});

foreach (var (nombre, ajeno) in new[] { ("Froststrap", true), ("froststrap clásico", true), ("Snowy", false), (null, false) })
{
    await Test($"marca: mod de la comunidad '{nombre}' oculto = {ajeno}", () =>
    {
        Equal(ajeno, Branding.IsForeignBrand(nombre));
        return Task.CompletedTask;
    });
}

Console.WriteLine(failures == 0 ? "\nTodas las pruebas pasan." : $"\n{failures} prueba(s) fallan.");
return failures == 0 ? 0 : 1;

sealed class FakeSource : ILaunchUpdateSource
{
    public List<string> Calls { get; } = [];
    public string[]? AppliedArgs { get; private set; }
    public bool IsInstalled { get; init; } = true;
    public string? Latest { get; init; }
    public Func<Task>? OnCheck { get; init; }
    public Func<CancellationToken, Task>? OnDownload { get; init; }
    public Action? OnApply { get; init; }

    public async Task<string?> FindUpdateAsync()
    {
        Calls.Add("check");

        if (OnCheck is not null)
            await OnCheck();

        return Latest;
    }

    public async Task DownloadAsync(CancellationToken cancellationToken)
    {
        Calls.Add("download");

        if (OnDownload is not null)
            await OnDownload(cancellationToken);
    }

    public void RecordAttempt(string version) => Calls.Add($"record {version}");

    public void ApplyAndRestart(string[] args)
    {
        Calls.Add("apply");
        AppliedArgs = args;
        OnApply?.Invoke();
    }
}
