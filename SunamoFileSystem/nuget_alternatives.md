# NuGet Alternatives to SunamoFileSystem

This document lists popular NuGet packages that provide similar functionality to SunamoFileSystem.

## Overview

File system utilities

## Alternative Packages

### System.IO
- **NuGet**: System.IO.FileSystem
- **Purpose**: Built-in file system APIs
- **Key Features**: File, Directory, Path, DriveInfo operations

### System.IO.Abstractions
- **NuGet**: System.IO.Abstractions
- **Purpose**: Mockable file system
- **Key Features**: IFileSystem interface, testing support

### Zio
- **NuGet**: Zio
- **Purpose**: Virtual file systems
- **Key Features**: In-memory FS, composable file systems

### Alphaleonis.Win32.Filesystem
- **NuGet**: AlphaFS
- **Purpose**: Extended Windows file operations
- **Key Features**: Long paths, transactional NTFS, advanced features

## Comparison Notes

System.IO for standard operations. AlphaFS for Windows-specific advanced features.

## Choosing an Alternative

Consider these alternatives based on your specific needs:
- **System.IO**: Built-in file system APIs
- **System.IO.Abstractions**: Mockable file system
- **Zio**: Virtual file systems
