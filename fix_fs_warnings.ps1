$filePath = 'E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoFileSystem\SunamoFileSystem\FS.cs'
$lines = [System.IO.File]::ReadAllLines($filePath)

# Parse all unique warnings from build output
$buildOutput = Get-Content 'E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoFileSystem\build_output.txt'
$seen = @{}
$warnings = @()
foreach ($line in $buildOutput) {
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
            $warnings += [PSCustomObject]@{Line=$lineNum; Col=$col; Code=$code; Msg=$msg}
        }
    }
}

Write-Host "Total unique warnings to fix: $($warnings.Count)"

# Group warnings by type
$byCode = $warnings | Group-Object Code | Sort-Object Name
foreach ($g in $byCode) {
    Write-Host "  $($g.Name): $($g.Count)"
}

# ==========================================
# FIX CS0168/CS0219: Remove unused variables
# ==========================================
$unusedVarLines = ($warnings | Where-Object { $_.Code -eq 'CS0168' -or $_.Code -eq 'CS0219' }).Line | Sort-Object -Descending
foreach ($lineNum in $unusedVarLines) {
    $idx = $lineNum - 1
    $line = $lines[$idx]
    # For catch blocks with unused exception variable, remove the variable name
    if ($line -match '^\s*catch\s*\(\s*(\w+)\s+(\w+)\s*\)') {
        $lines[$idx] = $line -replace 'catch\s*\(\s*(\w+)\s+\w+\s*\)', 'catch ($1)'
        Write-Host "Fixed CS0168 at line ${lineNum}: removed unused catch variable"
    }
    elseif ($line -match '^\s*(var|string|int|bool|long|double|float|char)\s+\w+\s*=') {
        # Remove the entire line for unused local variables
        $lines[$idx] = ''
        Write-Host "Fixed CS0219 at line ${lineNum}: removed unused variable declaration"
    }
}

# ==========================================
# FIX CS8600: Converting null to non-nullable type
# ==========================================
$cs8600Lines = ($warnings | Where-Object { $_.Code -eq 'CS8600' }).Line | Sort-Object
foreach ($lineNum in $cs8600Lines) {
    $idx = $lineNum - 1
    $line = $lines[$idx]

    # Pattern: path = Path.GetDirectoryName(path); -> path = Path.GetDirectoryName(path)!;
    if ($line -match 'Path\.GetDirectoryName\(' -and $line -notmatch '!;') {
        $lines[$idx] = $line -replace '(Path\.GetDirectoryName\([^)]+\))', '$1!'
        Write-Host "Fixed CS8600 at line ${lineNum}: added null-forgiving to GetDirectoryName"
    }
    # Pattern: var x = something that can be null
    elseif ($line -match '(=\s*)(null)\s*;') {
        # For lines that assign null, make the type nullable
        if ($line -match '^\s*(string|List<[^>]+>|Dictionary<[^>]+>|object|DateTime)\s+(\w+)\s*=\s*null') {
            $type = $Matches[1]
            $lines[$idx] = $line -replace "^(\s*)${type}\s+", "`$1${type}? "
            Write-Host "Fixed CS8600 at line ${lineNum}: made type nullable"
        }
    }
    elseif ($line -match '\.GetDirectoryName\(' -and $line -notmatch '\?') {
        $lines[$idx] = $line -replace '((?:Path|PathMs)\.GetDirectoryName\([^)]+\))', '$1!'
        Write-Host "Fixed CS8600 at line ${lineNum}: added null-forgiving to GetDirectoryName"
    }
}

# ==========================================
# FIX CS8602: Dereference of possibly null reference
# ==========================================
$cs8602Lines = ($warnings | Where-Object { $_.Code -eq 'CS8602' }).Line | Sort-Object
foreach ($lineNum in $cs8602Lines) {
    $idx = $lineNum - 1
    $line = $lines[$idx]

    # Line 20: Path.GetPathRoot(path).Equals(...)
    if ($line -match 'Path\.GetPathRoot\([^)]+\)\.') {
        $lines[$idx] = $line -replace '(Path\.GetPathRoot\([^)]+\))', '$1!'
        Write-Host "Fixed CS8602 at line ${lineNum}: added null-forgiving to GetPathRoot"
    }
    # Lines with .GetFileNameWithoutExtension().
    elseif ($line -match '\.GetFileNameWithoutExtension\([^)]+\)\.') {
        $lines[$idx] = $line -replace '(\.GetFileNameWithoutExtension\([^)]+\))', '$1!'
        Write-Host "Fixed CS8602 at line ${lineNum}: added null-forgiving to GetFileNameWithoutExtension"
    }
    # Pattern: variable.Something where variable could be null
    elseif ($line -match 'Path\.GetDirectoryName\([^)]+\)\.') {
        $lines[$idx] = $line -replace '(Path\.GetDirectoryName\([^)]+\))', '$1!'
        Write-Host "Fixed CS8602 at line ${lineNum}: added null-forgiving to GetDirectoryName"
    }
}

# ==========================================
# FIX CS8603: Possible null reference return
# ==========================================
$cs8603Lines = ($warnings | Where-Object { $_.Code -eq 'CS8603' }).Line | Sort-Object
foreach ($lineNum in $cs8603Lines) {
    $idx = $lineNum - 1
    $line = $lines[$idx]

    # For return statements with GetDirectoryName
    if ($line -match 'return\s+Path\.GetDirectoryName\(') {
        $lines[$idx] = $line -replace '(Path\.GetDirectoryName\([^)]+\))', '$1!'
        Write-Host "Fixed CS8603 at line ${lineNum}: added null-forgiving to return"
    }
    elseif ($line -match 'return\s+(\w+)\s*;' -and $line -notmatch 'return\s+null') {
        # Add ! to return value
        $lines[$idx] = $line -replace 'return\s+(\w+)\s*;', 'return $1!;'
        Write-Host "Fixed CS8603 at line ${lineNum}: added null-forgiving to return value"
    }
}

# ==========================================
# FIX CS8604: Possible null reference argument
# ==========================================
$cs8604Lines = ($warnings | Where-Object { $_.Code -eq 'CS8604' }).Line | Sort-Object
foreach ($lineNum in $cs8604Lines) {
    $idx = $lineNum - 1
    $line = $lines[$idx]

    # Path.Combine(Path.GetDirectoryName(...), ...)
    if ($line -match 'Path\.Combine\(Path\.GetDirectoryName\(') {
        $lines[$idx] = $line -replace '(Path\.GetDirectoryName\([^)]+\))', '$1!'
        Write-Host "Fixed CS8604 at line ${lineNum}: added null-forgiving to GetDirectoryName in Combine"
    }
    # Path.Combine(variable, ...) where variable might be null
    elseif ($line -match 'Path\.Combine\((\w+),') {
        $varName = $Matches[1]
        if ($varName -eq 'path' -or $varName -eq 'folder' -or $varName -eq 'dir') {
            $lines[$idx] = $line -replace "Path\.Combine\(${varName},", "Path.Combine(${varName}!,"
            Write-Host "Fixed CS8604 at line ${lineNum}: added null-forgiving to variable in Combine"
        }
    }
    # CreateFoldersPsysicallyUnlessThere(Path.GetDirectoryName(path))
    elseif ($line -match 'CreateFoldersPsysicallyUnlessThere\(Path\.GetDirectoryName\(') {
        $lines[$idx] = $line -replace '(Path\.GetDirectoryName\([^)]+\))', '$1!'
        Write-Host "Fixed CS8604 at line ${lineNum}: added null-forgiving to GetDirectoryName"
    }
    # foldersToCreate.Add(path) where path could be null
    elseif ($line -match '\.Add\(path\)') {
        $lines[$idx] = $line -replace '\.Add\(path\)', '.Add(path!)'
        Write-Host "Fixed CS8604 at line ${lineNum}: added null-forgiving to path"
    }
    # FilesOfExtension(folder, ...) where folder could be null
    elseif ($line -match 'FilesOfExtension\((\w+),') {
        $varName = $Matches[1]
        $lines[$idx] = $line -replace "FilesOfExtension\(${varName},", "FilesOfExtension(${varName}!,"
        Write-Host "Fixed CS8604 at line ${lineNum}: added null-forgiving"
    }
    # WithEndSlash(Path.GetDirectoryName(...))
    elseif ($line -match 'WithEndSlash\(Path\.GetDirectoryName\(') {
        $lines[$idx] = $line -replace '(Path\.GetDirectoryName\([^)]+\))', '$1!'
        Write-Host "Fixed CS8604 at line ${lineNum}: added null-forgiving"
    }
}

# ==========================================
# FIX CS8625: Cannot convert null to non-nullable
# ==========================================
$cs8625Lines = ($warnings | Where-Object { $_.Code -eq 'CS8625' }).Line | Sort-Object
foreach ($lineNum in $cs8625Lines) {
    $idx = $lineNum - 1
    $line = $lines[$idx]

    # For method parameters with default null: args = null -> args = null with nullable type
    if ($line -match ',\s*(GetExtensionArgs)\s+(\w+)\s*=\s*null') {
        $lines[$idx] = $line -replace '(GetExtensionArgs)\s+(\w+)\s*=\s*null', '$1? $2 = null'
        Write-Host "Fixed CS8625 at line ${lineNum}: made parameter nullable"
    }
    elseif ($line -match 'string\s+(\w+)\s*=\s*null\b') {
        $lines[$idx] = $line -replace 'string\s+(\w+)\s*=\s*null\b', 'string? $1 = null'
        Write-Host "Fixed CS8625 at line ${lineNum}: made string parameter nullable"
    }
    elseif ($line -match 'Func<[^>]+>\s+(\w+)\s*=\s*null\b') {
        $lines[$idx] = $line -replace '(Func<[^>]+>)\s+(\w+)\s*=\s*null\b', '$1? $2 = null'
        Write-Host "Fixed CS8625 at line ${lineNum}: made Func parameter nullable"
    }
    elseif ($line -match 'Action<[^>]+>\s+(\w+)\s*=\s*null\b') {
        $lines[$idx] = $line -replace '(Action<[^>]+>)\s+(\w+)\s*=\s*null\b', '$1? $2 = null'
        Write-Host "Fixed CS8625 at line ${lineNum}: made Action parameter nullable"
    }
}

# ==========================================
# FIX CS0618: Obsolete member usage
# ==========================================
$cs0618Lines = ($warnings | Where-Object { $_.Code -eq 'CS0618' }).Line | Sort-Object
foreach ($lineNum in $cs0618Lines) {
    $idx = $lineNum - 1
    $line = $lines[$idx]

    # string.Copy(x) -> new string(x)
    if ($line -match 'string\.Copy\(') {
        $lines[$idx] = $line -replace 'string\.Copy\(([^)]+)\)', 'new string($1)'
        Write-Host "Fixed CS0618 at line ${lineNum}: replaced string.Copy with new string"
    }
    # Obsolete method call - add annotation to suppress if it's our own method
    elseif ($line -match 'MoveAllFilesRecursively\(') {
        Write-Host "CS0618 at line ${lineNum}: Obsolete method call MoveAllFilesRecursively - needs manual review"
    }
}

# ==========================================
# FIX CS8714: Nullability of type argument doesn't match
# ==========================================
# Line 1270 - handled by making the generic constraint match

# ==========================================
# FIX CS1587: XML comment not on valid element
# ==========================================
$cs1587Lines = ($warnings | Where-Object { $_.Code -eq 'CS1587' }).Line | Sort-Object -Descending
foreach ($lineNum in $cs1587Lines) {
    $idx = $lineNum - 1
    # Find the start and end of the XML comment block
    $startIdx = $idx
    while ($startIdx -gt 0 -and $lines[$startIdx - 1] -match '^\s*///') {
        $startIdx--
    }
    $endIdx = $idx
    while ($endIdx -lt $lines.Count - 1 -and $lines[$endIdx + 1] -match '^\s*///') {
        $endIdx++
    }
    # Check if the next non-empty line after comment is a valid element
    $nextLineIdx = $endIdx + 1
    while ($nextLineIdx -lt $lines.Count -and $lines[$nextLineIdx].Trim() -eq '') {
        $nextLineIdx++
    }
    if ($nextLineIdx -lt $lines.Count) {
        $nextLine = $lines[$nextLineIdx]
        # If it's a commented-out method, remove the XML comment
        if ($nextLine -match '^\s*//' -or $nextLine -match '^\s*$' -or $nextLine -match '^\s*#') {
            for ($i = $startIdx; $i -le $endIdx; $i++) {
                $lines[$i] = ''
            }
            Write-Host "Fixed CS1587 at line ${lineNum}: removed orphaned XML comment"
        }
    }
}

# Save the file
[System.IO.File]::WriteAllLines($filePath, $lines)
Write-Host "`nFile saved."
