# SunamoFileSystem

## Short description

Rozsáhlá knihovna pro práci se souborovým systémem v .NET.


Overall working with filesystem

## Overview

SunamoFileSystem is part of the Sunamo package ecosystem, providing modular, platform-independent utilities for .NET development.

## Main Components

### Key Classes

- **AppPaths**
- **GetExtensionArgs**
- **FileSystemWatchers**
- **FS**

### Key Methods

- `GetStartupPath()`
- `GetFileInStartupPath()`
- `Start()`
- `Stop()`
- `IsAbsolutePath()`
- `CopyFolder()`
- `CreateUpfoldersPsysicallyUnlessThere()`
- `ExistsDirectory()`
- `CreateFoldersPsysicallyUnlessThere()`

## Installation

```bash
dotnet add package SunamoFileSystem
```

## Dependencies

- **Diacritics** (v3.3.29)
- **Microsoft.Extensions.Logging.Abstractions** (v9.0.3)

## Package Information

- **Package Name**: SunamoFileSystem
- **Version**: 25.10.3.4
- **Target Framework**: net9.0
- **Category**: Platform-Independent NuGet Package
- **Source Files**: 54

## Related Packages

This package is part of the Sunamo package ecosystem. For more information about related packages, visit the main repository.

## License

See the repository root for license information.

<!-- sunamo:test-coverage:start -->
## Test coverage

| Metric | Value |
|---|---|
| Method | static |
| Test projects | 1 |
| Public types referenced by tests | 7/20 (35.0 %) |
| Public methods referenced by tests | 28/190 (14.7 %) |
| Line coverage | n/a |
| Branch coverage | n/a |
| Generated (UTC) | 2026-11-22 21:24 |

<!-- sunamo:test-coverage:data {"package":"SunamoFileSystem","container":"pinp","testProjects":1,"publicTypes":20,"typesReferencedByTests":7,"publicMethods":190,"methodsReferencedByTests":28,"lineRate":null,"branchRate":null,"method":"static","generatedUtc":"2026-11-22 21:24"} -->
<!-- sunamo:test-coverage:end -->
