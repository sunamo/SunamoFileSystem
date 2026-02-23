param([string]$FileFilter = "")
$lines = Get-Content 'E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoFileSystem\build_output.txt'
$seen = @{}
foreach ($line in $lines) {
    if ($line -match '(.*?)\((\d+),(\d+)\): warning (CS\d+): (.*)') {
        $file = $Matches[1].Trim()
        $lineNum = $Matches[2]
        $col = $Matches[3]
        $code = $Matches[4]
        $msg = $Matches[5]
        # Deduplicate (warnings appear for each target framework)
        $dedupKey = "$file|$lineNum|$col|$code"
        if ($seen.ContainsKey($dedupKey)) { continue }
        $seen[$dedupKey] = $true
        if ($FileFilter -eq "" -or $file -like "*$FileFilter*") {
            Write-Output "${file}(${lineNum},${col}): ${code}: ${msg}"
        }
    }
}
