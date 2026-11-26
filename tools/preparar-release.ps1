# Deja en .build\release exactamente lo que hay que subir al release de GitHub:
#   - AveriStrap-Installer-vX.Y.Z.exe  (el puente: UNICO .exe, lo descargan la 1.1.4, la web y el bot)
#   - los .nupkg de esa version y releases.windows-x64.json  (los usa la actualizacion automatica)
# El Setup.exe, el .msi y el .zip NO se suben.
#
# Antes: cargo xtask publish
# Uso:   powershell -ExecutionPolicy Bypass -File tools\preparar-release.ps1
param(
    [string]$Version,
    [string]$VelopackDir,
    [string]$OutputDir,
    [string]$Puente   # puente ya compilado; si se omite, se compila aqui
)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
if (-not $VelopackDir) { $VelopackDir = Join-Path $root '.build\velopack' }
if (-not $OutputDir) { $OutputDir = Join-Path $root '.build\release' }

if (-not $Version) {
    $csproj = [xml](Get-Content (Join-Path $root 'AveriStrap\AveriStrap.csproj'))
    $Version = $csproj.Project.PropertyGroup.AppVersion.'#text' | Where-Object { $_ } | Select-Object -First 1
    if (-not $Version) { throw 'No se encontro <AppVersion> en AveriStrap.csproj' }
}

$paquetes = @(Get-ChildItem $VelopackDir -Filter "AveriStrap-$Version-windows-x64-*.nupkg")
if (-not ($paquetes | Where-Object Name -like '*-full.nupkg')) { throw "No hay paquete de la version $Version en $VelopackDir" }

$indice = Join-Path $VelopackDir 'releases.windows-x64.json'
if (-not (Test-Path $indice)) { throw "Falta $indice" }

if (-not $Puente) {
    $setup = Join-Path $VelopackDir 'AveriStrap-windows-x64-Setup.exe'
    if (-not (Test-Path $setup)) { throw "Falta $setup" }

    $compilado = Join-Path (Split-Path -Parent $OutputDir) 'puente'
    dotnet publish (Join-Path $root 'tools\puente-actualizacion') -c Release "-p:SetupPath=$setup" "-p:Version=$Version" -o $compilado
    if ($LASTEXITCODE -ne 0) { throw 'No se pudo compilar el puente' }
    $Puente = Join-Path $compilado 'AveriStrap-Installer.exe'
}

New-Item -ItemType Directory $OutputDir -Force | Out-Null
# fuera lo de intentos anteriores, para no subir archivos mezclados
Get-ChildItem $OutputDir -File | Where-Object { $_.Name -match '\.(exe|nupkg)$|^releases\..*\.json$' } | Remove-Item -Force

Copy-Item $Puente (Join-Path $OutputDir "AveriStrap-Installer-v$Version.exe")
$paquetes | Copy-Item -Destination $OutputDir
Copy-Item $indice $OutputDir

Write-Host "Listo. Sube al release v$Version solo estos archivos de $OutputDir :"
Get-ChildItem $OutputDir -File | ForEach-Object { Write-Host (" - {0} ({1:N1} MB)" -f $_.Name, ($_.Length / 1MB)) }
