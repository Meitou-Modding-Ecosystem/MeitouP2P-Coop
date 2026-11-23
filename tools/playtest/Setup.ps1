param([string]$Meitou, [string]$Kenshi, [Alias('Profile')][string]$ProfileDir = (Join-Path $PSScriptRoot 'profile'))
$ErrorActionPreference = 'Stop'
if (!$Meitou) { $Meitou = Read-Host 'Extracted Meitou 0.22.1 folder (contains MeitouClient.exe)' }
if (!$Kenshi) { $Kenshi = Read-Host 'Your Kenshi installation folder' }
$Meitou = (Resolve-Path -LiteralPath $Meitou).Path
$Kenshi = (Resolve-Path -LiteralPath $Kenshi).Path
$exe = (Get-Item -LiteralPath (Join-Path $Meitou 'MeitouClient.exe')).FullName
$gameVersion = (Get-Item -LiteralPath (Join-Path $Meitou 'MeitouClient.dll')).VersionInfo.ProductVersion
$package = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'preview.json') -Raw | ConvertFrom-Json
if ($gameVersion -ne $package.gameBuild) { throw "Use the matching stock Meitou release. Required $($package.gameBuild), found $gameVersion." }
$data = (Resolve-Path -LiteralPath (Join-Path $Kenshi 'data')).Path
$ProfileDir = [IO.Path]::GetFullPath($ProfileDir)
if (Test-Path -LiteralPath $ProfileDir) { throw "Profile already exists: $ProfileDir. Choose a new -ProfileDir folder to keep existing playtests." }
$install = Join-Path $ProfileDir 'install'
New-Item -ItemType Directory -Path $ProfileDir, "$ProfileDir\temp", "$install\mods" | Out-Null
New-Item -ItemType Junction -Path "$install\data" -Target $data | Out-Null
foreach ($name in 'resources.cfg','controls.cfg','fcs.def','screens.cfg','settings.cfg','kenshi.cfg','__tutorials.data','currentVersion.txt') {
    $source = Join-Path $Kenshi $name
    if (Test-Path -LiteralPath $source) { Copy-Item -LiteralPath $source -Destination $install }
}
foreach ($name in 'MeitouP2P','MeitouP2P-Coop','Stoat Walk') {
    Copy-Item -LiteralPath (Join-Path $PSScriptRoot "mods\$name") -Destination "$install\mods" -Recurse
}
@('MeitouP2P.mod','MeitouP2P-Coop.mod','Stoat Walk.mod') | Set-Content -LiteralPath "$ProfileDir\mods.cfg" -Encoding ascii
@{version=2; fullScreen=$false; vSync=$false; showNames=$true; splashScreens=$false; tutorials=$false; pauseOnLoad=$false; edgeScroll=$false; uiMode=1; meitouMainMenu=$false; gameplay=@{autoSave=$false}} |
    ConvertTo-Json -Depth 4 | Set-Content -LiteralPath "$ProfileDir\meitou.user.json" -Encoding UTF8
@{game=$exe; install=$install; gameBuild=$gameVersion} | ConvertTo-Json | Set-Content -LiteralPath "$ProfileDir\preview-settings.json" -Encoding UTF8
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'preview.json') -Destination $ProfileDir
Write-Output "Profile ready: $ProfileDir"
Write-Output "Launch with .\Launch.ps1 -ProfileDir '$ProfileDir'"
