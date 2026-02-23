$lines = Get-Content 'E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoFileSystem\build_output.txt'
$fileWarnings = @{}
foreach ($line in $lines) {
    if ($line -match '(.*?)\((\d+),(\d+)\): warning (CS\d+)') {
        $file = $Matches[1].Trim()
        $code = $Matches[4]
        $lineNum = $Matches[2]
        if (-not $fileWarnings.ContainsKey($file)) {
            $fileWarnings[$file] = [System.Collections.ArrayList]::new()
        }
        $null = $fileWarnings[$file].Add("$code`:$lineNum")
    }
}
foreach ($f in ($fileWarnings.Keys | Sort-Object)) {
    $codes = $fileWarnings[$f] | ForEach-Object { ($_ -split ':')[0] } | Group-Object | Sort-Object Name | ForEach-Object { "$($_.Name):$($_.Count)" }
    $summary = $codes -join ' '
    Write-Output "$f => $summary"
}
Write-Output ""
Write-Output "Total files: $($fileWarnings.Count)"
Write-Output "Total warnings: $(($fileWarnings.Values | ForEach-Object { $_.Count } | Measure-Object -Sum).Sum)"
