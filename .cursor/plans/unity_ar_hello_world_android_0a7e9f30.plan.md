---
name: Unity AR Minimal Android
overview: Guide user through creating a minimal Unity AR application for Android using Unity Editor tools, with plane detection and object placement functionality. Project will be saved to current workspace for future AI-assisted development.
todos: []
isProject: false
---

# Unity AR Minimal App for Android - Step-by-Step Guide

## Overview

Guide you through creating a minimal Unity AR application for Android using Unity Editor's standard tools. The app will detect horizontal surfaces (planes) and allow placing a simple 3D object via screen tap. This serves as a learning project to familiarize with Unity development tools and AR Foundation.

**Important**: This is a guided walkthrough - you'll use Unity Editor's GUI to create the project, and it will be saved to the current workspace (`C:\Repos\github\pm-unity\`) for future AI-assisted improvements.

## Project Structure

After following the guide, your Unity project will have this structure:

```
pm-unity/
├── Assets/
│   ├── Scenes/
│   │   └── ARScene.unity                # Main AR scene
│   ├── Scripts/
│   │   └── ARPlacementController.cs     # AR placement script
│   └── Prefabs/
│       └── ARObject.prefab              # Simple 3D object prefab (cube/sphere)
├── ProjectSettings/                      # Unity project settings (auto-generated)
└── Packages/                            # Unity packages (auto-generated)
```

## Step-by-Step Implementation Guide

### Step 1: Create Unity Project in Current Workspace

**In Unity Hub:**

- Click "New Project"
- Select "3D (Built-in Render Pipeline)" template
- **Important**: Set project location to `C:\Repos\github\pm-unity\` (or browse to current workspace)
- Name: `pm-unity` (or keep existing name)
- Click "Create project"

**What you'll learn**: Unity Hub project creation, workspace organization

### Step 2: Configure Project for Android

**In Unity Editor:**

- Go to `File > Build Settings`
- Select "Android" platform
- Click "Switch Platform" (wait for conversion)
- Click "Player Settings" button
- In Player Settings:
  - **Other Settings**:
    - Minimum API Level: Android 7.0 'Nougat' (API level 24) - required for ARCore
    - Target API Level: Automatic (highest installed)
    - Scripting Backend: IL2CPP
    - Target Architectures: ✓ ARM64 (required for ARCore)
  - **XR Plug-in Management**:
    - Click "Install XR Plugin Management" if prompted
    - Under "Android" tab, enable ✓ "ARCore"

**What you'll learn**: Platform switching, Android build configuration, XR settings

### Step 3: Install AR Foundation Packages

**In Unity Editor:**

- Go to `Window > Package Manager`
- Click dropdown: "Unity Registry" (not "In Project")
- Search for "AR Foundation" → Click "Install"
- Search for "ARCore XR Plugin" → Click "Install"
- Wait for packages to install

**What you'll learn**: Unity Package Manager, AR Foundation ecosystem

### Step 4: Create Folder Structure

**In Project window (bottom):**

- Right-click `Assets` folder → `Create > Folder`
- Create folders: `Scenes`, `Scripts`, `Prefabs`

**What you'll learn**: Unity project organization, folder structure

### Step 5: Create AR Scene

**In Unity Editor:**

- Go to `File > New Scene` → Select "Basic (Built-in)"
- Go to `File > Save As` → Save as `ARScene` in `Assets/Scenes/`
- In Hierarchy window (left):
  - Right-click → `XR > AR Session Origin`
  - Right-click → `XR > AR Session`
- Select "AR Session Origin" in Hierarchy:
  - In Inspector (right), verify it has "AR Camera" child
  - Add component: `AR Plane Manager` (Add Component → XR → AR Plane Manager)

**What you'll learn**: Scene creation, AR Foundation GameObjects, component system

### Step 6: Create Simple 3D Object Prefab

**In Unity Editor:**

- In Hierarchy, right-click → `3D Object > Cube` (or Sphere)
- Rename it to "ARObject"
- Position at (0, 0, 0) - this is just for prefab creation
- In Project window, drag "ARObject" from Hierarchy to `Assets/Prefabs/` folder
- Delete "ARObject" from Hierarchy (keep the prefab)

**What you'll learn**: Prefab creation, 3D object basics

### Step 7: Create AR Placement Script

**In Unity Editor:**

- In Project window, right-click `Assets/Scripts/` → `Create > C# Script`
- Name it `ARPlacementController`
- Double-click to open in Visual Studio 2026
- Write minimal script that:
  - Detects screen taps
  - Raycasts to AR planes
  - Instantiates prefab at hit location
- Save script and return to Unity (auto-compiles)
- In Hierarchy, select "AR Session Origin"
- In Inspector, click "Add Component" → Add `ARPlacementController` script
- Drag `ARObject` prefab from Project to script's "Object To Place" field

**What you'll learn**: C# scripting in Unity, MonoBehaviour pattern, component attachment, Inspector references

### Step 8: Configure Android Build Settings

**In Unity Editor:**

- Go to `Edit > Project Settings > Player`
- Under "Other Settings":
  - Package Name: `com.yourname.arapp` (change "yourname" to your name)
  - Version: 0.1
- Go to `Edit > Preferences > External Tools`
- Set Android SDK path (if not auto-detected):
  - Usually: `C:\Users\[YourUser]\AppData\Local\Android\Sdk`
  - Or install via Unity Hub: `Installs > Add Modules > Android Build Support`

**What you'll learn**: Android package naming, SDK configuration

### Step 9: Android Device Setup (One-Time)

**On your Android phone:**

1. Go to `Settings > About Phone`
2. Tap "Build Number" 7 times (enables Developer Options)
3. Go back to `Settings > Developer Options`
4. Enable "USB Debugging"
5. Connect phone to PC via USB
6. On phone, allow USB debugging when prompted

**In Unity Editor:**

- Go to `File > Build Settings`
- Click "Run Device" dropdown - your device should appear
- If not visible, check USB drivers or try different USB cable/port

**What you'll learn**: Android developer setup, device connection

### Step 10: Build and Test

**In Unity Editor:**

- Ensure `ARScene` is open and saved
- Go to `File > Build Settings`
- Verify "ARScene" is in "Scenes In Build" list (add if needed)
- Select your Android device from "Run Device" dropdown
- Click "Build And Run"
- Unity will build APK and install on device
- App should launch and show camera view
- Point camera at flat surface (floor/table)
- Tap screen to place object

**What you'll learn**: Build process, deployment workflow, AR testing

## Script Template for ARPlacementController.cs

The script you'll write in Step 7 should include:

- Reference to AR Raycast Manager (for raycasting to planes)
- Reference to prefab to instantiate
- Input handling (touch/tap detection)
- Raycast from screen tap to AR planes
- Instantiate prefab at hit location
- Basic error handling

## Key Concepts You'll Learn

- **Unity Editor Interface**: Scene view, Game view, Hierarchy, Inspector, Project window
- **GameObjects and Components**: How Unity's entity-component system works
- **AR Foundation**: AR Session Origin, AR Session, AR Plane Manager, AR Raycast Manager
- **Prefabs**: Reusable object templates
- **C# Scripting**: MonoBehaviour lifecycle, component references, input handling
- **Android Build Pipeline**: Platform switching, build settings, APK generation
- **Device Deployment**: USB debugging, direct deployment from Unity

## Troubleshooting Tips

- **AR not working**: Ensure device supports ARCore (check Google's ARCore device list)
- **Build fails**: Verify Android SDK path, check minimum API level
- **Device not detected**: Check USB debugging, try different USB cable
- **Script errors**: Check Console window (Window > General > Console) for compilation errors
- **Planes not detected**: Ensure good lighting, point camera at textured surfaces

## Next Steps After This Guide

Once you've completed the basic app:

- The project will be in your workspace for AI-assisted improvements
- You can add features like object rotation, multiple object types, UI buttons
- Experiment with AR features: anchors, face tracking, image tracking
- Optimize performance and add polish

## Technical Details

- **AR Framework**: AR Foundation (Unity's cross-platform AR framework)
- **AR Provider**: ARCore (Google's AR platform for Android)
- **Rendering**: Built-in Render Pipeline (simplest for beginners)
- **Scripting**: C# scripts using Unity's MonoBehaviour pattern
- **Minimal Functionality**: Plane detection + tap-to-place single object

## Why This Approach?

- **Hands-on Learning**: You'll use Unity Editor tools directly, building muscle memory
- **Standard Workflow**: Follows Unity's recommended practices
- **AI-Ready**: Project saved to workspace allows future AI assistance for improvements
- **Minimal but Complete**: Simple enough to understand, complete enough to be functional

## Prerequisites Check

Before starting, ensure:

- ✅ Unity Hub and Unity Editor 6.3 LTS are installed
- ⚠️ Android SDK (will install via Unity Hub if needed)
- ⚠️ Android device supports ARCore (check: https://developers.google.com/ar/discover/supported-devices)
- ✅ USB cable for device connection
- ✅ Visual Studio 2026 installed (for script editing)

## Notes

- All Unity-generated files (ProjectSettings, Packages, Library) will be in the workspace
- `.gitignore` is already configured to exclude build artifacts and temporary files
- After completing the guide, you can commit the project structure to git
- Future AI assistance can help improve scripts, add features, optimize performance