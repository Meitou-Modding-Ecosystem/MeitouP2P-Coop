param(
    [Parameter(Mandatory)][string]$Connector,
    [Parameter(Mandatory)][string]$Coop,
    [string]$Dotnet = 'dotnet'
)
$ErrorActionPreference = 'Stop'
function Invoke-Checked([string]$Command, [string[]]$Arguments) {
    & $Command @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$Command failed ($LASTEXITCODE): $Arguments" }
}
$Connector = (Resolve-Path -LiteralPath $Connector).Path
$Coop = (Resolve-Path -LiteralPath $Coop).Path
Invoke-Checked (Join-Path $Connector 'build.ps1') @('-MeitouDir', $env:MEITOU_DIR, '-CoopDir', $Coop, '-Dotnet', $Dotnet)
Invoke-Checked python @((Join-Path $Connector 'tools/smoke.py'), '--dotnet', $Dotnet)
