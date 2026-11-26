# Prueba de tools\preparar-release.ps1 con archivos falsos (no compila nada).
# Uso: powershell -ExecutionPolicy Bypass -File tests\preparar-release.test.ps1
$ErrorActionPreference = 'Stop'
$script = Join-Path (Split-Path -Parent $PSScriptRoot) 'tools\preparar-release.ps1'
$tmp = Join-Path ([IO.Path]::GetTempPath()) "averistrap-release-test-$([guid]::NewGuid().ToString('N'))"
$packs = Join-Path $tmp 'velopack'
$out = Join-Path $tmp 'release'
$fallos = 0

function Test-Caso([string]$nombre, [bool]$ok) {
    if ($ok) { Write-Host "OK     $nombre" } else { Write-Host "FALLA  $nombre"; $script:fallos++ }
}

function Invoke-Preparar([string]$version) {
    try {
        & $script -Version $version -VelopackDir $packs -OutputDir $out -Puente (Join-Path $tmp 'puente.exe') | Out-Null
        return $true
    } catch { Write-Host "       ($($_.Exception.Message))"; return $false }
}

try {
    New-Item -ItemType Directory $packs, $out | Out-Null
    'puente' | Set-Content (Join-Path $tmp 'puente.exe')
    foreach ($f in 'AveriStrap-9.9.9-windows-x64-full.nupkg', 'AveriStrap-9.9.9-windows-x64-delta.nupkg',
        'AveriStrap-9.9.8-windows-x64-full.nupkg', 'AveriStrap-windows-x64-Setup.exe', 'AveriStrap-windows-x64.msi',
        'AveriStrap-windows-x64-Portable.zip', 'releases.windows-x64.json') { $f | Set-Content (Join-Path $packs $f) }
    'viejo' | Set-Content (Join-Path $out 'AveriStrap-Installer-v9.9.8.exe')

    Test-Caso 'el script termina bien' (Invoke-Preparar '9.9.9')

    $esperados = 'AveriStrap-9.9.9-windows-x64-delta.nupkg,AveriStrap-9.9.9-windows-x64-full.nupkg,AveriStrap-Installer-v9.9.9.exe,releases.windows-x64.json'
    $hay = (Get-ChildItem $out -ErrorAction SilentlyContinue | Sort-Object Name | ForEach-Object Name) -join ','
    Test-Caso "quedan solo los archivos del release (hay: $hay)" ($hay -eq $esperados)

    $exe = Join-Path $out 'AveriStrap-Installer-v9.9.9.exe'
    Test-Caso 'el unico .exe es el puente' ((Test-Path $exe) -and (Get-Content $exe) -eq 'puente')

    Test-Caso 'falla si no hay paquete de esa version' (-not (Invoke-Preparar '9.9.7'))

    Remove-Item (Join-Path $packs 'releases.windows-x64.json')
    Test-Caso 'falla si falta releases.windows-x64.json' (-not (Invoke-Preparar '9.9.9'))
} finally {
    Remove-Item $tmp -Recurse -Force -ErrorAction SilentlyContinue
}

if ($fallos) { Write-Host "`n$fallos prueba(s) fallan."; exit 1 }
Write-Host "`nTodas las pruebas pasan."
