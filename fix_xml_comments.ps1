# Fix XML comment warnings in FS.cs
# CS1572: XML comment has a param tag for X, but there is no parameter by that name
# CS1573: Parameter X has no matching param tag
# CS1591: Missing XML comment for publicly visible type or member

$filePath = 'E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoFileSystem\SunamoFileSystem\FS.cs'
$lines = [System.IO.File]::ReadAllLines($filePath)

# Parse warnings from build output
$buildOutput = Get-Content 'E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoFileSystem\build_output2.txt'
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
        if ($file.EndsWith('\FS.cs') -and $code -in @('CS1572','CS1573','CS1591')) {
            $warnings += [PSCustomObject]@{Line=$lineNum; Col=$col; Code=$code; Msg=$msg}
        }
    }
}

Write-Host "XML comment warnings to fix: $($warnings.Count)"

# ==========================================
# FIX CS1572: Wrong param tag name in XML comment
# We need to find the param tag and rename it to match the actual parameter
# ==========================================
$cs1572Warnings = $warnings | Where-Object { $_.Code -eq 'CS1572' } | Sort-Object Line -Descending
foreach ($w in $cs1572Warnings) {
    $lineNum = $w.Line
    $idx = $lineNum - 1
    # Extract the wrong param name from warning message
    if ($w.Msg -match "param tag for '(\w+)'") {
        $wrongName = $Matches[1]
        # Find the line with the wrong param tag
        $foundIdx = -1
        for ($i = $idx; $i -ge [Math]::Max(0, $idx - 15); $i--) {
            if ($lines[$i] -match "///\s*<param\s+name=""${wrongName}""") {
                $foundIdx = $i
                break
            }
        }
        if ($foundIdx -ne -1) {
            # Remove the entire param tag line
            $lines[$foundIdx] = ''
            Write-Host "Fixed CS1572 at line ${lineNum}: removed wrong param tag '${wrongName}'"
        }
    }
}

# ==========================================
# FIX CS1573: Parameter has no matching param tag
# We need to add the missing param tag, or if there's a wrong one, this was already handled
# ==========================================
$cs1573Warnings = $warnings | Where-Object { $_.Code -eq 'CS1573' } | Sort-Object Line -Descending
foreach ($w in $cs1573Warnings) {
    $lineNum = $w.Line
    $idx = $lineNum - 1
    # Extract the missing param name
    if ($w.Msg -match "Parameter '(\w+)' has no matching") {
        $missingParam = $Matches[1]
        # Find the method declaration line
        $methodLine = $lines[$idx]
        # Look for existing XML comments above
        $commentEnd = $idx - 1
        while ($commentEnd -ge 0 -and $lines[$commentEnd].Trim() -eq '') { $commentEnd-- }

        $hasXmlComment = $false
        $lastParamLineIdx = -1
        $summaryEndIdx = -1
        $commentStartIdx = -1

        if ($commentEnd -ge 0 -and $lines[$commentEnd].Trim() -match '^\s*///') {
            $hasXmlComment = $true
            # Find the start of the XML comment and last param tag
            $commentStartIdx = $commentEnd
            while ($commentStartIdx -gt 0 -and $lines[$commentStartIdx - 1].Trim() -match '^\s*///') {
                $commentStartIdx--
            }
            for ($i = $commentStartIdx; $i -le $commentEnd; $i++) {
                if ($lines[$i] -match '///\s*<param\s') { $lastParamLineIdx = $i }
                if ($lines[$i] -match '///\s*</summary>') { $summaryEndIdx = $i }
            }
        }

        if ($hasXmlComment) {
            # Get the indentation from existing comment
            $indent = ''
            if ($lines[$commentStartIdx] -match '^(\s*)///') { $indent = $Matches[1] }

            # Determine a meaningful description based on param name
            $desc = switch -Regex ($missingParam) {
                '^logger$' { 'Logger instance for diagnostic messages.' }
                '^path$' { 'The file or directory path.' }
                '^files\d*$' { 'List of file paths.' }
                '^folder\d*$' { 'The directory path.' }
                '^so$' { 'Specifies whether to search the current directory or all subdirectories.' }
                '^overwrite$' { 'Whether to overwrite existing files.' }
                '^value$' { 'The value to process.' }
                '^text$' { 'The text to process.' }
                '^filePath$' { 'The file path.' }
                '^fileName$' { 'The file name.' }
                '^newExt$' { 'The new file extension.' }
                '^physically$' { 'Whether to perform the operation physically on disk.' }
                '^collisionOption$' { 'How to handle file name collisions.' }
                '^directoryCollisionOption$' { 'How to handle directory name collisions.' }
                '^fileCollisionOption$' { 'How to handle file name collisions.' }
                '^move$' { 'Whether to move (true) or copy (false) files.' }
                '^args$' { 'Arguments controlling the operation behavior.' }
                '^EncodingHelperIsBinary$' { 'Function to determine if a file is binary.' }
                '^searchPattern$' { 'The search pattern to match against file names.' }
                '^searchOption$' { 'Specifies whether to search the current directory or all subdirectories.' }
                '^extension$' { 'The file extension.' }
                '^parts$' { 'The path parts to combine.' }
                '^terminateProcessIfIsInUsed$' { 'Whether to terminate the process if the file is in use.' }
                '^falseIfSizeZeroOrEmpty$' { 'If true, returns false for zero-size or empty files.' }
                '^a1IsWithPath$' { 'Whether the first argument includes a path.' }
                '^serieStyle$' { 'The serie naming style to use.' }
                '^serie$' { 'The output serie number found.' }
                '^fromUnit$' { 'The source unit of measurement.' }
                '^filesFull$' { 'List of full file paths.' }
                default { "The ${missingParam} parameter." }
            }

            $newParamLine = "${indent}/// <param name=""${missingParam}"">${desc}</param>"

            # Insert after the last param tag, or after summary if no params
            $insertAfter = if ($lastParamLineIdx -ne -1) { $lastParamLineIdx }
                          elseif ($summaryEndIdx -ne -1) { $summaryEndIdx }
                          else { $commentEnd }

            $linesList = [System.Collections.ArrayList]::new($lines)
            $linesList.Insert($insertAfter + 1, $newParamLine)
            $lines = $linesList.ToArray()
            Write-Host "Fixed CS1573 at line ${lineNum}: added param tag for '${missingParam}'"
        }
    }
}

# ==========================================
# FIX CS1591: Missing XML comment
# ==========================================
# Re-read lines since we may have inserted lines above
# Build a map from original line numbers to current line numbers

# Actually, let's re-parse from current state since CS1591 is about adding new comments
# We need to rebuild since line numbers shifted

[System.IO.File]::WriteAllLines($filePath, $lines)
Write-Host "`nIntermediate save done. Now fixing CS1591..."

# Re-read the file
$lines = [System.IO.File]::ReadAllLines($filePath)

# Rebuild to get new line numbers for CS1591
$tempBuild = & dotnet build --no-incremental "E:\vs\Projects\PlatformIndependentNuGetPackages\SunamoFileSystem\SunamoFileSystem\SunamoFileSystem.csproj" 2>&1
$tempBuildStr = $tempBuild | Out-String

$seen2 = @{}
$cs1591Warnings = @()
foreach ($line in ($tempBuild -split "`n")) {
    if ($line -match '(.*?)\((\d+),(\d+)\): warning CS1591: (.*)') {
        $file = $Matches[1].Trim()
        $lineNum = [int]$Matches[2]
        $col = [int]$Matches[3]
        $msg = $Matches[4] -replace '\s*\[.*$', ''
        $dedupKey = "${file}|${lineNum}|${col}"
        if ($seen2.ContainsKey($dedupKey)) { continue }
        $seen2[$dedupKey] = $true
        if ($file.EndsWith('\FS.cs')) {
            $cs1591Warnings += [PSCustomObject]@{Line=$lineNum; Msg=$msg}
        }
    }
}

Write-Host "CS1591 warnings to fix: $($cs1591Warnings.Count)"

# Sort descending so insertions don't shift earlier line numbers
$cs1591Warnings = $cs1591Warnings | Sort-Object Line -Descending

foreach ($w in $cs1591Warnings) {
    $lineNum = $w.Line
    $idx = $lineNum - 1
    $memberLine = $lines[$idx]

    # Skip if there's already an XML comment above
    $prevIdx = $idx - 1
    while ($prevIdx -ge 0 -and $lines[$prevIdx].Trim() -eq '') { $prevIdx-- }
    if ($prevIdx -ge 0 -and $lines[$prevIdx].Trim() -match '^\s*///') { continue }

    # Determine indentation
    $indent = ''
    if ($memberLine -match '^(\s*)') { $indent = $Matches[1] }

    # Parse the member name and type from warning message
    $memberName = ''
    $memberType = 'member'
    if ($w.Msg -match "'FS\.(\w+)'$") {
        $memberName = $Matches[1]
    }
    elseif ($w.Msg -match "'FS\.(\w+)\(") {
        $memberName = $Matches[1]
    }
    elseif ($w.Msg -match "'FS\.(\w+)<") {
        $memberName = $Matches[1]
    }

    # Determine what kind of member it is
    $isMethod = $memberLine -match '\('
    $isField = $memberLine -match '^\s*(public|protected|internal)\s+static\s+(readonly\s+)?(string|List|Dictionary|Action|Func|bool|int|long|char|double)' -and !$isMethod
    $isProperty = $memberLine -match '\{\s*get' -or $memberLine -match '=>'
    $isEnum = $memberLine -match '^\s*public\s+enum\s'

    # Generate meaningful summary based on member name
    $summary = switch -Regex ($memberName) {
        '^DEndsWithReplaceInFile$' { 'Default file name pattern for ends-with replacement operations.' }
        '^invalidFileNameCharsReadonly$' { 'Read-only list of invalid file name characters from the OS.' }
        '^invalidFileNameStringsReadonly$' { 'Read-only list of invalid file name characters as strings.' }
        '^invalidPathChars$' { 'List of invalid path characters including directory separators.' }
        '^InvalidFileNameCharsString$' { 'All invalid file name characters concatenated as a single string.' }
        '^InvalidFileNameChars$' { 'List of all invalid file name characters including extended Unicode.' }
        '^invalidCharsForMapPath$' { 'List of invalid characters for path mapping operations.' }
        '^invalidFileNameCharsWithoutDelimiterOfFolders$' { 'Invalid file name characters excluding directory separators.' }
        '^ReplaceIncorrectFor$' { 'Replacement string for incorrect file name characters.' }
        '^DeleteFileMaybeLocked$' { 'Action delegate to delete a file that may be locked by another process.' }
        '^FileUtilWhoIsLocking$' { 'Function to determine which processes are locking a file.' }
        '^IsAbsolutePath$' { 'Determines whether the specified path is an absolute path.' }
        '^ExistsDirectory$' { 'Determines whether the specified directory exists on disk.' }
        '^ExistsFile$' { 'Determines whether the specified file exists on disk.' }
        '^CreateDirectoryIfNotExists$' { 'Creates a directory if it does not already exist.' }
        '^WithEndSlash$' { 'Ensures the path ends with a trailing backslash.' }
        '^FoldersWithSubfolder$' { 'Finds all folders that contain a specific subfolder.' }
        '^TryDeleteFile$' { 'Attempts to delete a file, returning success status.' }
        '^CreateFileIfDoesntExists$' { 'Creates an empty file if it does not already exist.' }
        '^InsertBetweenFileNameAndExtension$' { 'Inserts text between the file name and its extension.' }
        '^IsExtension$' { 'Determines whether the specified string is a valid file extension.' }
        '^MoveSubfoldersToFolder$' { 'Moves specified subfolders from one location to another.' }
        '^TrimBasePathAndTrailingBs$' { 'Removes the base path and trailing backslashes from a list of paths.' }
        '^GetFileNameWithoutOneExtension$' { 'Gets the file name by removing the last path segment.' }
        '^GetActualDateTime$' { 'Returns the current date and time as a file-safe string.' }
        '^KeepOnlyWhichIsNotInFiles$' { 'Filters a list to keep only items not found in the specified files.' }
        '^FilterInRootAndInSubFolder$' { 'Separates files into root-level and subfolder groups.' }
        '^OnlyNames$' { 'Replaces full paths with just file names in the list.' }
        '^FilesWhichContainsAll$' { 'Finds files matching a pattern that contain all required contents.' }
        '^PathSpecialAndLevel$' { 'Constructs a path from base and relative segments up to a specified depth level.' }
        '^GetDirectoryNameIfIsFile$' { 'Returns the directory name if the path points to a file, otherwise returns the path.' }
        '^MaskFromExtensions$' { 'Creates a wildcard search mask from a list of file extensions.' }
        '^CopyMoveFilesInListSimple$' { 'Simplified version of CopyMoveFilesInList for basic copy/move operations.' }
        '^CreateInOtherLocationSameFolderStructure$' { 'Recreates the folder structure from one location in another.' }
        '^ReplaceInAllFiles$' { 'Replaces text in all specified files.' }
        '^RemoveDiacriticInFileContents$' { 'Removes diacritic marks from file contents in the specified folder.' }
        '^RemoveFile$' { 'Removes a file from disk.' }
        '^MakeFromLastPartFile$' { 'Creates a file from the last part of a path with the specified extension.' }
        '^CopyAs0KbFilesSubfolders$' { 'Copies the folder structure as empty 0KB files including subfolders.' }
        '^CopyAs0KbFiles$' { 'Copies files as empty 0KB files to the target directory.' }
        '^ShrinkLongPath$' { 'Shortens a long file path to a displayable length.' }
        '^CreateNewFolderPathWithEndingNextTo$' { 'Creates a new folder path adjacent to the specified path.' }
        '^CopyFilesOfExtensions$' { 'Copies files with specified extensions from source to target directory.' }
        '^GetUpFolderWhichContainsExtension$' { 'Traverses up the directory tree to find a folder containing files with the specified extension.' }
        '^TrimContentInFilesOfFolder$' { 'Trims whitespace from the content of all matching files in a folder.' }
        '^GetSizeIn$' { 'Converts a file size value between different computer size units.' }
        '^CompareTWithInt$' { 'Compares two TWithInt instances by their count in descending order.' }
        '^DirectoriesWithToken$' { 'Gets directories sorted by a numeric token in their path.' }
        '^MoveAllFilesRecursively$' { 'Moves all files recursively from source to target directory.' }
        '^SortPathsByFileName$' { 'Sorts file paths alphabetically by their file name.' }
        '^DeleteAllRecursively$' { 'Deletes all files and optionally directories recursively.' }
        '^DeleteFoldersWhichNotContains$' { 'Deletes folders that do not contain files matching specified patterns.' }
        '^OnlyExtensions$' { 'Extracts only the extensions from a list of file paths.' }
        '^OnlyExtensionsToLower$' { 'Extracts extensions from file paths and converts them to lowercase.' }
        '^OnlyExtensionsToLowerWithPath$' { 'Extracts lowercase extensions from file paths, preserving the path context.' }
        '^OnlyExtensionToLowerWithPath$' { 'Gets the lowercase extension of a single file path.' }
        '^AllExtensionsInFolders$' { 'Gets all unique file extensions found in the specified folders.' }
        '^ExpandEnvironmentVariables$' { 'Expands a known environment variable to its value.' }
        '^AddUpfoldersToRelativePath$' { 'Prepends parent directory references to a relative path.' }
        '^NormalizeExtension$' { 'Normalizes a file extension to ensure it starts with a dot.' }
        '^GetNormalizedExtension$' { 'Gets the normalized extension from a file path.' }
        '^ModifiedinUnix$' { 'Gets the last modified time of a file as a Unix timestamp.' }
        '^ReplaceDiacriticRecursive$' { 'Recursively removes diacritic marks from file and folder names.' }
        '^SaveStream$' { 'Saves a stream to a file at the specified path.' }
        '^OnlyNamesWithoutExtensionCopy$' { 'Returns a new list containing only file names without extensions.' }
        '^DirectoryExistsAndIsNotEmpty$' { 'Checks if a directory exists and contains files or subdirectories.' }
        '^OnlyNamesWithoutExtension$' { 'Replaces full paths with file names without extensions.' }
        '^Postfix$' { 'Appends a postfix to a file name before the extension.' }
        '^ReadAllText$' { 'Reads all text content from a file.' }
        '^GetFileNameWithoutExtension$' { 'Gets the file name from a path without the extension.' }
        '^ThrowNotImplementedUwp$' { 'Throws a not-implemented exception for UWP-specific operations.' }
        '^IsFileOlderThanXHours$' { 'Determines whether a file is older than the specified number of hours.' }
        '^GetFileNamesWoExtension$' { 'Gets file names without extensions from a list of paths.' }
        '^GetTokens$' { 'Splits a path into its component tokens.' }
        '^CopyStream$' { 'Copies data from one stream to another.' }
        '^SaveMemoryStream$' { 'Saves a memory stream to a file.' }
        '^DeleteWrongCharsInDirectoryName$' { 'Removes invalid characters from a directory name.' }
        '^DeleteWrongCharsInFileName$' { 'Removes invalid characters from a file name.' }
        '^ContainsInvalidPathCharForPartOfMapPath$' { 'Checks if a path segment contains invalid characters for mapping.' }
        '^CreateDirectory$' { 'Creates a directory with collision handling options.' }
        '^StreamToArrayBytes$' { 'Converts a stream to a byte array.' }
        '^AddExtensionIfDontHave$' { 'Adds a file extension if the path does not already have one.' }
        '^ChangeFilename$' { 'Changes the file name of a path, optionally renaming the physical file.' }
        '^TryDeleteDirectory$' { 'Attempts to delete a directory, catching any exceptions.' }
        '^GetSizeInAutoString$' { 'Returns a human-readable file size string with automatic unit selection.' }
        '^DirectoryListing$' { 'Lists files in a directory matching a pattern.' }
        '^WithoutEndSlash$' { 'Removes trailing backslash from a path.' }
        '^MascFromExtension$' { 'Creates a wildcard search mask from a file extension.' }
        '^IsCountOfFilesMoreThan$' { 'Checks if the number of files matching a pattern exceeds a threshold.' }
        '^GetFiles$' { 'Gets files from a directory, optionally including subdirectories.' }
        '^CopyMoveFilePrepare$' { 'Prepares source and target paths for a copy or move operation with collision handling.' }
        '^GetFileSize$' { 'Gets the size of a file in bytes.' }
        '^CopyAllFilesRecursively$' { 'Copies all files recursively from source to target directory.' }
        '^CopyFile$' { 'Copies a file from source to destination.' }
        '^LastModified$' { 'Gets the last modified date and time of a file.' }
        '^TryDeleteDirectoryOrFile$' { 'Attempts to delete a path whether it is a file or directory.' }
        '^AllIncludeIfOnlyLetters$' { 'Wraps a search term with wildcards if it contains only letters.' }
        '^ExistsDirectoryNull$' { 'Returns null if the directory does not exist.' }
        '^ToSearchOption$' { 'Converts a nullable boolean to a SearchOption value.' }
        '^WriteAllText$' { 'Writes text content to a file.' }
        '^IsAllInSameFolder$' { 'Determines if all paths in the list are in the same folder.' }
        '^CreateFileWithTemplateContent$' { 'Creates a file with content based on a template with placeholder replacement.' }
        '^ContainsInvalidFileNameChars$' { 'Checks if a string contains invalid file name characters.' }
        '^NumberByDateModified$' { 'Numbers files by their modification date.' }
        '^GetDirectoryName$' { 'Gets the directory name from a full path.' }
        '^DetectPathDelimiter$' { 'Detects the path delimiter used in a path string.' }
        '^DetectPathDelimiterChar$' { 'Detects the path delimiter character used in a path string.' }
        '^MakeUncLongPath$' { 'Converts a path to UNC long path format to support paths longer than 260 characters.' }
        '^FirstCharUpper$' { 'Converts the first character of a path to uppercase.' }
        '^GetPathAndFileName$' { 'Splits a file path into directory path and file name components.' }
        '^GetFileName$' { 'Gets the file name from a full path.' }
        '^NormalizeExtension2$' { 'Normalizes a file extension with additional processing.' }
        '^RemoveSerieUnderscore$' { 'Removes the underscore-style series suffix from a file name.' }
        '^DeleteFile$' { 'Deletes a file from disk, attempting to handle locked files.' }
        '^PathWithoutExtension$' { 'Returns the full path without the file extension.' }
        '^GetFullPath$' { 'Gets the fully qualified path for a relative path.' }
        '^FileToDirectory$' { 'Converts a file path to its parent directory path.' }
        '^FilesWhichSurelyExists$' { 'List of files that are confirmed to exist on disk.' }
        '^GetNameWithoutSeries$' { 'Extracts the base name from a file name by removing series suffixes.' }
        '^GetNameWithoutSeriesNoOut$' { 'Extracts the base name from a file name by removing series suffixes without out parameters.' }
        '^GetFolderSize$' { 'Calculates the total size of all files in a folder.' }
        '^GroupFilesByName$' { 'Groups file paths by their file name.' }
        '^BasePath$' { 'Finds which base path contains the specified path.' }
        '^HasAnyFoldersOrFiles$' { 'Determines if a folder contains any files or subdirectories.' }
        '^MoveDirectoryNoRecursive$' { 'Moves a directory without recursion.' }
        '^Combine$' { 'Combines multiple path segments into a single path.' }
        '^CombineFile$' { 'Combines multiple path segments into a file path.' }
        '^CombineDir$' { 'Combines multiple path segments into a directory path.' }
        default { "Performs the ${memberName} operation." }
    }

    # Build the XML comment
    $commentLines = @()
    $commentLines += "${indent}/// <summary>"
    $commentLines += "${indent}/// ${summary}"
    $commentLines += "${indent}/// </summary>"

    # Add param tags for methods
    if ($isMethod) {
        # Parse parameters from the method signature
        # Need to handle multi-line signatures
        $fullSig = $memberLine
        $openParens = ($fullSig.ToCharArray() | Where-Object { $_ -eq '(' }).Count
        $closeParens = ($fullSig.ToCharArray() | Where-Object { $_ -eq ')' }).Count
        $nextIdx = $idx + 1
        while ($openParens -gt $closeParens -and $nextIdx -lt $lines.Count) {
            $fullSig += ' ' + $lines[$nextIdx].Trim()
            $openParens = ($fullSig.ToCharArray() | Where-Object { $_ -eq '(' }).Count
            $closeParens = ($fullSig.ToCharArray() | Where-Object { $_ -eq ')' }).Count
            $nextIdx++
        }

        # Extract parameter names from signature
        if ($fullSig -match '\((.+)\)') {
            $paramStr = $Matches[1]
            # Split by comma, but be aware of generic type params
            $depth = 0
            $params = @()
            $current = ''
            foreach ($c in $paramStr.ToCharArray()) {
                if ($c -eq '<') { $depth++ }
                elseif ($c -eq '>') { $depth-- }
                elseif ($c -eq ',' -and $depth -eq 0) {
                    $params += $current.Trim()
                    $current = ''
                    continue
                }
                $current += $c
            }
            if ($current.Trim()) { $params += $current.Trim() }

            foreach ($param in $params) {
                $param = $param.Trim()
                # Skip empty params
                if (!$param) { continue }
                # Extract parameter name (last word, possibly with default value)
                $paramParts = $param -split '\s+'
                if ($paramParts.Count -ge 2) {
                    $paramName = $paramParts[-1] -replace '=.*$', '' -replace '[)\]]$', ''
                    # Skip 'this' keyword for extension methods
                    if ($paramName -eq 'this') { continue }
                    # Skip ref/out/params modifiers
                    $paramName = $paramName -replace '^(ref|out|params)\s+', ''
                    if ($paramName -and $paramName -ne 'this') {
                        $paramDesc = switch -Regex ($paramName) {
                            '^logger$' { 'Logger instance for diagnostic messages.' }
                            '^path$' { 'The file or directory path.' }
                            '^filePath$' { 'The file path.' }
                            '^folder\d*$' { 'The directory path.' }
                            '^text$' { 'The text to process.' }
                            '^value$' { 'The value to process.' }
                            '^collisionOption$' { 'How to handle collisions.' }
                            default { "The ${paramName} parameter." }
                        }
                        $commentLines += "${indent}/// <param name=""${paramName}"">${paramDesc}</param>"
                    }
                }
            }
        }
    }

    # Insert the comment above the member
    $linesList = [System.Collections.ArrayList]::new($lines)
    for ($i = $commentLines.Count - 1; $i -ge 0; $i--) {
        $linesList.Insert($idx, $commentLines[$i])
    }
    $lines = $linesList.ToArray()
    Write-Host "Fixed CS1591 at line ${lineNum}: added XML comment for '${memberName}'"
}

[System.IO.File]::WriteAllLines($filePath, $lines)
Write-Host "`nFile saved with all XML comment fixes."
