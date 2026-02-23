$lines = Get-Content 'E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoFileSystem\build_output5.txt'
$seen = @{}
$total = 0
$byCode = @{}
$byFile = @{}
foreach ($line in $lines) {
    if ($line -match '(.*?)\((\d+),(\d+)\): warning (CS\d+)') {
        $file = $Matches[1].Trim()
        $lineNum = [int]$Matches[2]
        $col = [int]$Matches[3]
        $code = $Matches[4]
        $dedupKey = "${file}|${lineNum}|${col}|${code}"
        if ($seen.ContainsKey($dedupKey)) { continue }
        $seen[$dedupKey] = $true
        $total++
        if (-not $byCode.ContainsKey($code)) { $byCode[$code] = 0 }
        $byCode[$code]++
        $shortFile = $file -replace '.*\\SunamoFileSystem\\SunamoFileSystem\\', ''
        if (-not $byFile.ContainsKey($shortFile)) { $byFile[$shortFile] = 0 }
        $byFile[$shortFile]++
    }
}
Write-Host "Total unique warnings: ${total}"
Write-Host ""
Write-Host "By code:"
$byCode.GetEnumerator() | Sort-Object Name | ForEach-Object { Write-Host "  $($_.Name): $($_.Value)" }
Write-Host ""
Write-Host "By file:"
$byFile.GetEnumerator() | Sort-Object Name | ForEach-Object { Write-Host "  $($_.Name): $($_.Value)" }
