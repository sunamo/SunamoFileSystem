$lines = Get-Content 'E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoFileSystem\build_output4.txt'
$seen = @{}
foreach ($line in $lines) {
    if ($line -match '(.*?)\((\d+),(\d+)\): warning (CS\d+): (.*)') {
        $file = $Matches[1].Trim()
        $lineNum = [int]$Matches[2]
        $col = [int]$Matches[3]
        $code = $Matches[4]
        $msg = $Matches[5] -replace '\s*\[.*$', ''
        $dedupKey = "${file}|${lineNum}|${col}|${code}"
        if ($seen.ContainsKey($dedupKey)) { continue }
        $seen[$dedupKey] = $true
        if ($file.EndsWith('\FS.cs')) {
            Write-Output "${lineNum}|${code}|${msg}"
        }
    }
}
