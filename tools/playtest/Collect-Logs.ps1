param([Alias('Profile')][string]$ProfileDir = (Join-Path $PSScriptRoot 'profile'))
$ErrorActionPreference = 'Stop'
$ProfileDir = (Resolve-Path -LiteralPath $ProfileDir).Path
$files = @(Get-ChildItem -LiteralPath $ProfileDir -File | Where-Object { $_.Extension -eq '.log' -or $_.Name -eq 'preview.json' })
$output = Join-Path $PSScriptRoot ('stoat-logs-' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.zip')
Compress-Archive -LiteralPath $files.FullName -DestinationPath $output
Write-Output "Logs collected at $output. Review them before sharing; logs can contain local paths and connection addresses."
