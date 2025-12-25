# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

LeoChrome is a lightweight Chromium-based web browser built with WPF (Windows Presentation Foundation). The browser emphasizes lower memory usage compared to Google Chrome while maintaining the same Edge Chromium rendering engine. Key features include real-time resource monitoring and YouTube video downloading capabilities.

## Technology Stack

- **Framework**: .NET 8.0 (Windows)
- **UI Framework**: WPF with MahApps.Metro 2.4.10 for modern UI
- **Browser Engine**: Microsoft WebView2 1.0.2651.64 (Edge Chromium)
- **Layout System**: Xceed AvalonDock for tabbed browser interface
- **Video Download**: YoutubeExplode 6.3.16 for YouTube video extraction
- **Behaviors**: Microsoft.Xaml.Behaviors.Wpf 1.1.122
- **Architecture Pattern**: MVVM (Model-View-ViewModel)

## Build Commands

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 (17.8+) or JetBrains Rider 2023.3+
- Windows 10/11

### Restore NuGet Packages
```bash
dotnet restore MahAppBase.sln
```

### Build Solution
```bash
# Build with .NET CLI
dotnet build MahAppBase.sln -c Debug
dotnet build MahAppBase.sln -c Release

# Or build specific project
dotnet build MahAppBase/MahAppBase.csproj -c Debug
```

### Run the Application
```bash
# Run directly
dotnet run --project MahAppBase/MahAppBase.csproj

# Or run the built executable
MahAppBase\bin\Debug\net8.0-windows\MahAppBase.exe
```

### Publish for Distribution
```bash
# Framework-dependent (requires .NET 8 Runtime)
dotnet publish MahAppBase/MahAppBase.csproj -c Release -o ./publish

# Self-contained (larger file size, no runtime required)
dotnet publish MahAppBase/MahAppBase.csproj -c Release -r win-x64 --self-contained true -o ./publish
```

**Important**: The `Sound` folder is automatically copied to the build output directory via the project configuration.

## Architecture

### MVVM Structure

The application follows MVVM pattern with clear separation:

- **Views (XAML)**:
  - `MainWindow.xaml` - Main browser window with AvalonDock layout
  - `UcPageContent.xaml` - Individual browser tab user control containing ChromiumWebBrowser
  - `UcDownLoadSetting.xaml` - Download settings dialog

- **ViewModels**:
  - `MainComponent` (MahAppBase\ViewModel\MainComponent.cs:24) - Main window view model managing tab collection, flyout panels, and window state
  - `LayoutDocument` (MahAppBase\ViewModel\LayoutDocument.cs:19) - Individual tab view model handling URL navigation, YouTube detection, and download functionality
  - `ViewModelBase` - Base class providing INotifyPropertyChanged implementation

- **Commands**:
  - `CommonCommand` - Generic command with parameter support
  - `NoParameterCommand` - Parameterless command implementation

### Key Components

**MainWindow Integration** (MahAppBase\MainWindow.xaml.cs:26):
- Inherits from `MetroWindow` for modern UI styling
- Hosts `LayoutDocumentPane` (named `MainGroup`) for dynamic tab management
- DataContext bound to `MainComponent` instance

**Tab Management** (MahAppBase\ViewModel\MainComponent.cs:359-372):
- Tabs are dynamically created via `ButtonNewTabClick` command
- Each tab is a `UcPageContent` user control wrapped in AvalonDock's `LayoutDocument`
- Delegates events from child tabs back to main window (URL loaded, keyboard input)
- Maintains `SubViewModelList` to track all tab ViewModels

**Browser Tab** (MahAppBase\CustomerUserControl\UcPageContent.xaml.cs:25):
- Uses Microsoft's `WebView2` control (named `cwUrl`)
- URL bar (`tbUrl`) with Enter key navigation
- Handles both direct URLs and Google search queries
- Updates tab title and tooltip from browser title changes via NavigationCompleted event
- Properly disposes WebView2 resources on close
- Asynchronously initializes WebView2 using EnsureCoreWebView2Async

**YouTube Download** (MahAppBase\ViewModel\LayoutDocument.cs:38-54):
- Automatically detects YouTube URLs via pattern matching (`youtube` + `watch?v=`)
- Shows download button when single video detected
- Shows playlist download button for playlist URLs (feature in development)
- Uses YoutubeExplode for video extraction (async/await pattern)
- Threading model: downloads run on background Task to avoid UI blocking
- Progress tracking with `IProgress<double>` and binding to `CurrentProgress`/`ProgressMax`
- Automatically selects highest quality muxed stream (video + audio)

**Performance Optimization** (MahAppBase\ViewModel\MainComponent.cs:188-205):
- `BetterPerformance` property controls rendering behavior
- When window minimized, collapses DockingManager visibility to reduce resource usage
- Responsive UI adapts download toolbar visibility based on window width (<900px threshold)

### Navigation Flow

1. User enters URL in `tbUrl` TextBox and presses Enter
2. `UIElement_OnKeyDown` handler checks if URL contains "http"
3. If yes: loads URL directly in `ChromiumWebBrowser`
4. If no: constructs Google search URL and loads it
5. On title change: updates tab title, tooltip, and ViewModel URL property
6. If URL is YouTube video: shows download button dynamically

### FlyOut Panels

The application uses MahApps.Metro FlyOut panels for settings and donation information:
- Panels slide in from right with 400px width animation
- `WebMargin` property adjusts main content area when FlyOut opens
- Mutually exclusive: opening one closes the other
- 3-second slide animation controlled via XAML Storyboards

## Common Issues

### Missing Sound Files
Error: "找不到路徑XXXXButton.wav的一部分"
**Solution**: Copy the `Sound` folder from project root to the build output directory (e.g., `bin\Debug\Sound\`)

### WebView2 Runtime
WebView2 requires the Edge WebView2 Runtime to be installed on the target machine:
- Windows 11: Pre-installed
- Windows 10: May need to be installed (auto-installed by installer or downloaded from Microsoft)

The application will automatically download the runtime if not present.

### Folder Dialog Compatibility
The application uses `OpenFolderDialog` (.NET 8 native), which requires:
- Windows 10 version 1809 or later
- For older Windows versions, consider using a third-party dialog library

## Project Structure Notes

- **RootNamespace**: `MahAppBase` (legacy name, actual project is LeoChrome)
- **Target Framework**: .NET Framework 4.8
- **Output Type**: Windows Application (WinExe)
- **Key Directories**:
  - `Command/` - ICommand implementations
  - `ViewModel/` - ViewModels and business logic
  - `CustomerUserControl/` - Custom user controls for tabs and dialogs
  - `Converter/` - Value converters for XAML bindings
  - `Interface/` - Interface definitions
  - `Resources/` - Icons, fonts, and XAML resource dictionaries
  - `Sound/` - Audio files for UI feedback

## Development Notes

- The YouTube playlist download feature is marked as "開發中" (in development) and currently shows a placeholder message
- Hard-coded download path in `LayoutDocument.cs:192` should be made configurable
- Event delegation pattern used throughout: child controls raise events that parent window handles
- Memory management: explicitly disposes CefSharp browsers and clears bindings on tab close
