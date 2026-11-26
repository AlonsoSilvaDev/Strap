namespace AveriStrap.Puente;

/// <summary>Carpetas implicadas en el paso de AveriStrap 1.x a 2.x.</summary>
/// <param name="OldRoot">Instalación antigua según el registro.</param>
/// <param name="InstallRoot">Donde instala Velopack (%LocalAppData%\AveriStrap).</param>
/// <param name="DataRoot">Datos de la versión nueva (%AppData%\AveriStrap).</param>
public sealed record Folders(string OldRoot, string InstallRoot, string DataRoot)
{
    public string NewExe => Path.Combine(InstallRoot, "current", "averistrap.exe");
}

public static class Migration
{
    // Solo lo que tiene el mismo formato en las dos versiones. Los ajustes no se importan.
    private static readonly string[] UserFolders = ["Modifications", "CustomThemes"];

    /// <summary>
    /// Copia los datos compatibles, aparta la instalación antigua e instala la nueva.
    /// No borra nada: si la instalación falla, deja todo como estaba.
    /// </summary>
    /// <param name="install">Ejecuta el instalador y devuelve true si terminó bien.</param>
    public static void Run(Folders folders, Func<bool> install)
    {
        if (File.Exists(folders.NewExe))
            return;

        foreach (string name in UserFolders)
            CopyMissing(Path.Combine(folders.OldRoot, name), Path.Combine(folders.DataRoot, name));

        // el instalador necesita la carpeta libre
        string? backup = null;

        if (Directory.Exists(folders.InstallRoot))
        {
            backup = FreeName(folders.InstallRoot + "-respaldo-1x");
            Directory.Move(folders.InstallRoot, backup);
        }

        try
        {
            if (!install() || !File.Exists(folders.NewExe))
                throw new InvalidOperationException("El instalador no terminó correctamente.");
        }
        catch
        {
            if (backup is not null)
            {
                if (Directory.Exists(folders.InstallRoot))
                    Directory.Move(folders.InstallRoot, FreeName(folders.InstallRoot + "-fallido"));

                Directory.Move(backup, folders.InstallRoot);
            }

            throw;
        }
    }

    private static string FreeName(string path)
    {
        string candidate = path;

        for (int i = 2; Directory.Exists(candidate) || File.Exists(candidate); i++)
            candidate = $"{path}-{i}";

        return candidate;
    }

    // nunca pisa un archivo que ya exista en el destino
    private static void CopyMissing(string source, string destination)
    {
        if (!Directory.Exists(source))
            return;

        Directory.CreateDirectory(destination);

        foreach (string file in Directory.GetFiles(source))
        {
            string target = Path.Combine(destination, Path.GetFileName(file));

            if (!File.Exists(target))
                File.Copy(file, target);
        }

        foreach (string directory in Directory.GetDirectories(source))
            CopyMissing(directory, Path.Combine(destination, Path.GetFileName(directory)));
    }
}
