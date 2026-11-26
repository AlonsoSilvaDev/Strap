// Puente de actualización: AveriStrap 1.x descarga el .exe del último release y lo abre
// con "-upgrade". Este programa es ese .exe: instala la versión 2.x y la abre.
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using AveriStrap.Puente;
using Microsoft.Win32;

const string Name = "AveriStrap";
const uint IconError = 0x10;

try
{
    string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    string installRoot = Path.Combine(local, Name);

    using var key = Registry.CurrentUser.OpenSubKey($@"Software\Microsoft\Windows\CurrentVersion\Uninstall\{Name}");

    var folders = new Folders(
        key?.GetValue("InstallLocation") as string ?? installRoot,
        installRoot,
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Name));

    if (!WaitForOtherCopies(TimeSpan.FromSeconds(20)))
        throw new InvalidOperationException("Cierra AveriStrap y Roblox, y vuelve a abrir AveriStrap para actualizar.");

    Migration.Run(folders, RunSetup);

    var start = new ProcessStartInfo(folders.NewExe);

    foreach (string arg in args)
        start.ArgumentList.Add(arg);

    Process.Start(start);
    return 0;
}
catch (Exception ex)
{
    MessageBox(IntPtr.Zero, $"No se pudo actualizar AveriStrap.\n\n{ex.Message}", Name, IconError);
    return 1;
}

static bool WaitForOtherCopies(TimeSpan timeout)
{
    var watch = Stopwatch.StartNew();

    while (Process.GetProcessesByName(Name).Any(p => p.Id != Environment.ProcessId))
    {
        if (watch.Elapsed > timeout)
            return false;

        Thread.Sleep(500);
    }

    return true;
}

static bool RunSetup()
{
    using Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("Setup.exe")
        ?? throw new InvalidOperationException("Este actualizador se compiló sin el instalador dentro.");

    string path = Path.Combine(Path.GetTempPath(), $"AveriStrap-Setup-{Guid.NewGuid():N}.exe");

    using (var file = File.Create(path))
        resource.CopyTo(file);

    try
    {
        using var process = Process.Start(new ProcessStartInfo(path, "--silent"))!;
        process.WaitForExit();
        return process.ExitCode == 0;
    }
    finally
    {
        try { File.Delete(path); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

[DllImport("user32.dll", CharSet = CharSet.Unicode)]
static extern int MessageBox(IntPtr window, string text, string caption, uint type);
