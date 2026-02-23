param([string]$FileFilter = "", [switch]$Exclude)
$lines = Get-Content 'E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoFileSystem\build_output.txt'
$seen = @{}
foreach ($line in $lines) {
    if ($line -match '(.*?)\((\d+),(\d+)\): warning (CS\d+): (.*)') {
        $file = $Matches[1].Trim()
        $lineNum = $Matches[2]
        $col = $Matches[3]
        $code = $Matches[4]
        $msg = $Matches[5]
        $dedupKey = "${file}|${lineNum}|${col}|${code}"
        if ($seen.ContainsKey($dedupKey)) { continue }
        $seen[$dedupKey] = $true
        $match = ($FileFilter -eq "") -or ($file -like "*${FileFilter}*")
        if ($Exclude) { $match = -not $match }
        if ($match) {
            Write-Output "${file}(${lineNum},${col}): ${code}: ${msg}"
        }
    }
}
