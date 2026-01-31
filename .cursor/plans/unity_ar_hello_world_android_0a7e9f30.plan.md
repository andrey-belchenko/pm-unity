# Unity AR Minimal App for Android - Step-by-Step Guide

## Overview

Use Unity's **AR Mobile** project template to create a minimal AR app for Android. The template pre-configures the project (URP, AR Foundation, XR Interaction Toolkit, ARCore), includes a **SampleScene** with plane detection and object spawning, and is ready for mobile AR. You will use Unity Hub and the Editor only—no custom scripts or manual AR setup.

**Important**: Save the project to the current workspace (`C:\Repos\github\pm-unity\`) so you can iterate on it with AI later.

**References (all verified from official Unity docs):**

- [AR Mobile Template Quick Start](https://docs.unity3d.com/Packages/com.unity.template.ar-mobile@2.0/manual/index.html)
- [Create an XR project](https://docs.unity3d.com/6000.1/Documentation/Manual/xr-create-projects.html)
- [Configure your AR development environment](https://learn.unity.com/tutorial/configure-your-ar-development-environment) (Unity Learn)
- [Build your application for Android](https://docs.unity3d.com/6000.1/Documentation/Manual/android-BuildProcess.html)
- [Debug on Android devices](https://docs.unity3d.com/6000.1/Documentation/Manual/android-debugging-on-an-android-device.html)
- [Choose and configure XR provider plug-ins](https://docs.unity3d.com/6000.1/Documentation/Manual/xr-configure-providers.html)

---

## What the AR Mobile Template Provides

The template configures the project and includes a ready-made scene. You do **not** install AR Foundation or ARCore manually, or create XR Origin / AR Session from scratch.

- **Render pipeline**: URP (Universal Render Pipeline)
- **Packages**: AR Foundation, XR Interaction Toolkit, ARKit XR Plug-in, ARCore XR Plug-in
- **Scene**: `SampleScene` in `Assets/Scenes`
- **XR Origin (AR Rig)**: AR Plane Manager, AR Raycast Manager, Input Action Manager, Main Camera (AR Camera Manager, AR Camera Background), Screen Space Ray Interactor (touchscreen gestures)
- **AR Session**: AR Session, AR Input Manager
- **Object Spawner**: Spawns XR Interactables via `ARInteractionSpawnTrigger`
- **Screen Space UI**: Create menu (spawn objects), Options modal (e.g. Show Interaction Hints, Visualize Surfaces, Remove All Objects, AR Debug Menu)

**Minimal AR functionality** = use `SampleScene` as-is: detect planes, open the create menu, tap to place objects. You can optionally strip example assets later (see below).

---

## Project Structure (After Creating from Template)

```
pm-unity/
├── Assets/
│   ├── Scenes/
│   │   └── SampleScene.unity
│   └── MobileARTemplateAssets/     (optional; can delete for minimal setup)
├── ProjectSettings/
└── Packages/
```

---

## Step-by-Step Guide

### Step 1: Create Project with AR Mobile Template

**In Unity Hub:**

1. Click **New Project**.
2. Select the **AR Mobile** template.

  - If you don’t see it, click **Download template** when prompted.

3. Set **Project location** to `C:\Repos\github\pm-unity\` (or your workspace root).

  - Ensure you’re not creating a subfolder like `pm-unity\pm-unity`; the project root should be the workspace.

4. Set **Project name** (e.g. `pm-unity`) and click **Create project**.

**What you’ll use:** Unity Hub, project creation, workspace layout.

---

### Step 2: Add Android Support (If Missing)

The template supports iOS and Android. You need the **Android** module for building to your phone.

- If **Android** is missing when you add a build profile or switch platform: in Unity Hub go to **Installs** → your Unity version → **Add modules** → enable **Android Build Support** and install.  
- See [Create an XR project](https://docs.unity3d.com/6000.1/Documentation/Manual/xr-create-projects.html) and [Build your application for Android](https://docs.unity3d.com/6000.1/Documentation/Manual/android-BuildProcess.html).

---

### Step 3: Configure XR Plug-in Management for Android

1. **Edit → Project Settings**.
2. Open **XR Plug-in Management**.
3. If you see **Install XR Plug-in Management**, click it.
4. Select the **Android** tab (Android icon).
5. Enable **ARCore** (and any other providers you need).

See [Choose and configure XR provider plug-ins](https://docs.unity3d.com/6000.1/Documentation/Manual/xr-configure-providers.html). Use **Project Validation** in the same section to fix any reported issues.

---

### Step 4: Switch to Android and Configure Player Settings

1. **File → Build Profiles** (Unity 6).

  - If your version uses **File → Build Settings**, use that instead; the flow is similar.

2. Add an **Android** build profile if you don’t have one (e.g. **Add Build Profile** → **Platform Browser** → **Android** → **Add Build Profile**).
3. **Switch** to the Android profile.
4. Open **Edit → Project Settings → Player**.
5. Under **Other Settings** (and any Android-specific overrides):

  - **Package Name**: e.g. `com.yourname.arapp`.
  - **Version**: e.g. `0.1`.

6. Set **Minimum API Level** and **Target API Level** as required for ARCore (typically minimum API 24). Use **Project Validation** and ARCore package docs if unsure.
7. **Edit → Preferences → External Tools**: set **Android SDK** path if Unity doesn’t detect it.

  - You can also install the SDK via Unity Hub (**Add modules** when adding the Android module).

See [Build your application for Android](https://docs.unity3d.com/6000.1/Documentation/Manual/android-BuildProcess.html) and [Android SDK setup](https://docs.unity3d.com/6000.1/Documentation/Manual/android-sdksetup.html).

---

### Step 5: (Optional) Test in Editor with XR Simulation

The AR Mobile template supports **XR Simulation** so you can test in the Editor without a device:

1. **Edit → Project Settings → XR Plug-in Management**.
2. Set **Desktop** plug-in provider to **XR Simulation** and enable **Initialize XR on Startup**.
3. **File → Build Profiles** → switch to **Android** (XR Simulation works better with Android/iOS as build target; see template docs).
4. Enter **Play** mode.
5. In the Game view: **Right‑click + move** to rotate; **Right‑click + W/A/S/D/Q/E** to move.

**Note:** With XR Simulation, on-screen UI may not appear correctly until the build target is Android (or iOS). See the [AR Mobile Template Quick Start](https://docs.unity3d.com/Packages/com.unity.template.ar-mobile@2.0/manual/index.html#xr-simulation).

---

### Step 6: Connect Your Android Device

1. On the phone: **Settings → About Phone** → tap **Build Number** 7 times to enable **Developer Options**.
2. **Settings → Developer Options** → enable **USB Debugging**.
3. Connect the device via USB. On first connect, allow **USB debugging** when prompted.
4. On Windows, install a device-specific USB driver if the device isn’t recognized.

See Unity’s [Debug on Android devices](https://docs.unity3d.com/6000.1/Documentation/Manual/android-debugging-on-an-android-device.html) and Android’s [Configure developer options](https://developer.android.com/studio/debug/dev-options) / [Set up a device for development](https://developer.android.com/studio/run/device#setting-up).

---

### Step 7: Build and Run on Device

1. **File → Build Profiles** (or **Build Settings**).
2. Select the **Android** build profile.
3. Ensure **SampleScene** is in **Scenes In Build** (add it if missing).
4. Set **Run Device** to your connected phone (use **Refresh** if it doesn’t appear).
5. Click **Build And Run**. Choose an output path when prompted; Unity builds the APK and installs it on the device.

See [Build your application for Android](https://docs.unity3d.com/6000.1/Documentation/Manual/android-BuildProcess.html) and [Debug on Android devices](https://docs.unity3d.com/6000.1/Documentation/Manual/android-debugging-on-an-android-device.html).

---

### Step 8: Use the App (Minimal AR Flow)

- Open the app on the device. Point the camera at a flat, textured surface (e.g. floor, table).
- When planes are detected, use the **Create** menu (e.g. bottom button), pick an object, then tap on a plane to place it.
- Use the **Options** menu (e.g. top-right) for interaction hints, **Visualize Surfaces**, **Remove All Objects**, or **AR Debug Menu** as needed.

This is the minimal AR flow provided by the template—no custom code required.

---

## Optional: Minimal Scene by Removing Example Assets

If you want a **minimal** scene with only AR basics (no template UI/assets):

1. In the **Project** window, under **Assets**, right‑click **MobileARTemplateAssets** → **Delete** → confirm.
2. Remove any **disconnected script** from the **XR Origin** GameObject (the template docs mention “ARSessionOrigin XROrigin object”) to avoid errors.

See [AR Mobile Template – Removing the example Assets](https://docs.unity3d.com/Packages/com.unity.template.ar-mobile@2.0/manual/index.html#removing-the-example-assets-from-the-scene). After this, you still have XR Origin, AR Session, and plane detection; you’d add your own spawn logic or UI if needed.

---

## Troubleshooting

- **AR / tracking not working**: Confirm device supports [ARCore](https://developers.google.com/ar/discover/supported-devices). Check XR Plug-in Management → Android → ARCore enabled.
- **Build fails**: Check Android SDK path, API levels, and **Project Validation** under XR Plug-in Management.
- **Device not listed**: Verify USB debugging, cable, and drivers. Try **Refresh** in Build Profiles / Run Device.
- **Planes not detected**: Use adequate lighting and textured surfaces.
- **Samsung S23 Ultra**: Template [known issue](https://docs.unity3d.com/Packages/com.unity.template.ar-mobile@2.0/manual/index.html#known-issues): tap input can behave oddly when spawning objects nearby.

---

## What You’ll Learn

- **Unity Hub**: Creating a project from a template, setting location and name.
- **AR Mobile template**: Preconfigured URP, AR Foundation, ARCore, SampleScene, XR Origin, Object Spawner, UI.
- **Editor**: Project Settings, XR Plug-in Management, Player settings, Build Profiles (or Build Settings).
- **Android**: Switching platform, Run Device, Build and Run, basic device setup (USB debugging).
- **Minimal AR**: Plane detection + tap-to-place using the template’s SampleScene.

---

## Prerequisites

- Unity Hub and Unity Editor (e.g. 6.3 LTS) installed.
- Android module installed (via Hub) if you build to device.
- Android phone with [ARCore support](https://developers.google.com/ar/discover/supported-devices), USB cable, and USB debugging enabled.
- (Optional) Visual Studio for editing scripts when you extend the project.

---

## Next Steps

- Keep the project in `C:\Repos\github\pm-unity\` for version control and AI-assisted changes.
- Use **Configure your AR development environment** on Unity Learn for a more detailed walkthrough.
- After you’re comfortable, remove example assets for a minimal scene, or add your own objects and UI.