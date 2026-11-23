param([Alias('Profile')][string]$ProfileDir = (Join-Path $PSScriptRoot 'profile'), [string]$Size = '1280x800', [switch]$TestControl, [string]$CacheDir)
$ErrorActionPreference = 'Stop'
$ProfileDir = (Resolve-Path -LiteralPath $ProfileDir).Path
$settings = Get-Content -LiteralPath "$ProfileDir\preview-settings.json" -Raw | ConvertFrom-Json
if ($Size -notmatch '^\d{3,4}x\d{3,4}$') { throw 'Use a window size such as 1280x800.' }
$version = (Get-Item -LiteralPath (Join-Path (Split-Path $settings.game) 'MeitouClient.dll')).VersionInfo.ProductVersion
if ($version -ne $settings.gameBuild) { throw 'The selected Meitou installation changed. Prepare a fresh matching profile.' }
if (Test-Path -LiteralPath "$ProfileDir\client.pid") {
    $old = Get-Process -Id ([int](Get-Content -LiteralPath "$ProfileDir\client.pid")) -ErrorAction SilentlyContinue
    if ($old -and $old.ProcessName -eq 'MeitouClient') { throw 'This playtest profile already has a running client.' }
}
if (!$CacheDir) { $CacheDir = if ($settings.cache) { $settings.cache } else { Join-Path $PSScriptRoot 'cache' } }
$CacheDir = [IO.Path]::GetFullPath($CacheDir)
New-Item -ItemType Directory -Path $CacheDir -Force | Out-Null
$names = @('KENSHI_PATH','MEITOU_MODS_CFG','MEITOU_STOAT_WALK_ROLE','MEITOU_STOAT_WALK_PORT','MEITOU_HEADLESS','DOTNET_PROCESSOR_COUNT','TEMP','TMP','MEITOU_OBJECT_LOD_CACHE','MEITOU_CHARACTER_LOD_CACHE','MEITOU_IMPOSTOR_CACHE')
$saved = @{}
foreach ($name in $names) { $saved[$name] = [Environment]::GetEnvironmentVariable($name, 'Process') }
try {
    $env:KENSHI_PATH = $settings.install
    $env:MEITOU_MODS_CFG = "$ProfileDir\mods.cfg"
    $env:MEITOU_STOAT_WALK_ROLE = $null
    $env:MEITOU_STOAT_WALK_PORT = $null
    $env:MEITOU_HEADLESS = $null
    $env:DOTNET_PROCESSOR_COUNT = '4'
    $env:TEMP = "$ProfileDir\temp"
    $env:TMP = $env:TEMP
    # Keep inherited per-cache overrides from bypassing the selected trial cache.
    $env:MEITOU_OBJECT_LOD_CACHE = $null
    $env:MEITOU_CHARACTER_LOD_CACHE = $null
    $env:MEITOU_IMPOSTOR_CACHE = $null
    $arguments = "--windowed --size $Size --seed 17 --time 15 --fps-limit 30 --no-audio --no-gi --no-precompile --shadow-quality 0 --texture-quality 1 --water-reflection 0"
    $arguments += ' --cache-root "' + ($CacheDir -replace '(\\+)$', '$1$1') + '"'
    if ($TestControl) {
        New-Item -ItemType Directory -Path "$ProfileDir\control" -Force | Out-Null
        $arguments += ' --control-dir "' + "$ProfileDir\control" + '"'
    }
    $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
    $client = Start-Process -FilePath $settings.game -ArgumentList $arguments -WorkingDirectory $ProfileDir -WindowStyle Normal -PassThru -RedirectStandardOutput "$ProfileDir\client-$stamp.log" -RedirectStandardError "$ProfileDir\errors-$stamp.log"
    $client.Id | Set-Content -LiteralPath "$ProfileDir\client.pid" -Encoding ascii
    Write-Output "Client PID $($client.Id). Choose Multiplayer in the main menu."
    Write-Output "Derived-data cache: $CacheDir"
}
finally { foreach ($name in $names) { [Environment]::SetEnvironmentVariable($name, $saved[$name], 'Process') } }
