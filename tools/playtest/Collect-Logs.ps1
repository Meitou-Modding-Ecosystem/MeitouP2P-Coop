param([Alias('Profile')][string]$ProfileDir = (Join-Path $PSScriptRoot 'profile'))
$ErrorActionPreference = 'Stop'
$ProfileDir = (Resolve-Path -LiteralPath $ProfileDir).Path
$files = @(Get-ChildItem -LiteralPath $ProfileDir -File | Where-Object { $_.Extension -eq '.log' -or $_.Name -eq 'preview.json' })
$output = Join-Path $PSScriptRoot ('stoat-logs-' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff') + '.zip')
Add-Type -AssemblyName System.IO.Compression
$archiveStream = [IO.File]::Open($output, [IO.FileMode]::CreateNew, [IO.FileAccess]::Write, [IO.FileShare]::None)
$archive = $null
try {
    $archive = [IO.Compression.ZipArchive]::new($archiveStream, [IO.Compression.ZipArchiveMode]::Create, $true)
    foreach ($file in $files) {
        # Start-Process keeps stdout/stderr open. Snapshot the current bytes with
        # read/write sharing; Compress-Archive can skip these live logs.
        $logStream = [IO.File]::Open($file.FullName, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::ReadWrite)
        try {
            $entry = $archive.CreateEntry($file.Name).Open()
            try {
                $remaining = $logStream.Length
                $buffer = New-Object byte[] 65536
                while ($remaining -gt 0) {
                    $read = $logStream.Read($buffer, 0, [int][Math]::Min($buffer.Length, $remaining))
                    if ($read -eq 0) { throw "Log ended before its captured length: $($file.Name)" }
                    $entry.Write($buffer, 0, $read)
                    $remaining -= $read
                }
            } finally { $entry.Dispose() }
        } finally { $logStream.Dispose() }
    }
} finally {
    if ($archive) { $archive.Dispose() }
    $archiveStream.Dispose()
}
Write-Output "Logs collected at $output. Review them before sharing; logs can contain local paths and connection addresses."
