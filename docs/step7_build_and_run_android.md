# Step 7: Build and Run on Device - Detailed Guide

## Overview

This guide provides detailed instructions for building and running your Unity AR app on an Android device using Unity's Build Profiles (or Build Settings in older versions).

---

## Step-by-Step Instructions

### 1. Open Build Profiles/Build Settings

- **Unity 6**: Go to **File → Build Profiles**
- **Older Unity versions**: Go to **File → Build Settings**

### 2. Select Android Build Profile

- In the Build Profiles window, select the **Android** build profile
- If you don't have an Android build profile yet:
  - Click **Add Build Profile**
  - Select **Platform Browser**
  - Choose **Android**
  - Click **Add Build Profile**

### 3. Add SampleScene to Scenes In Build

- In the Build Profiles window, locate the **Scenes In Build** section
- Ensure `SampleScene` is listed there
- If `SampleScene` is missing:
  - Open the **Project** window
  - Navigate to `Assets/Scenes/`
  - Drag `SampleScene.unity` into the **Scenes In Build** list
  - Alternatively, if the scene is already open in the Editor, click **Add Open Scenes**

### 4. Set Run Device

- In the Build Profiles window, find the **Run Device** dropdown
- Connect your Android phone to your computer via USB (make sure USB debugging is enabled)
- Click **Refresh** if your device doesn't appear in the list
- Select your device from the dropdown

**Troubleshooting device not appearing:**
- Ensure USB debugging is enabled on your phone (Settings → Developer Options → USB Debugging)
- Try a different USB cable or USB port
- On Windows, you may need to install device-specific USB drivers
- Check if your device is recognized: Open PowerShell and run `adb devices` to see if your device is listed

### 5. Build and Run

- Click **Build And Run** (or **Build** if you want to install manually later)
- When prompted, choose an output path for the APK (e.g., `Builds/` folder)
- Unity will:
  - Build the APK file
  - Install it on your connected device
  - Launch the app automatically

---

## What to Expect

- The first build may take several minutes as Unity compiles all assets and scripts
- Once the build completes, the app should automatically launch on your device
- Point your device's camera at a flat, textured surface (like a floor or table)
- When planes are detected, use the **Create** menu to place objects

---

## Common Issues and Troubleshooting

### Build Fails

- **Check Android SDK path**: Go to **Edit → Preferences → External Tools** and verify the Android SDK path is set correctly
- **Verify API levels**: Ensure your Minimum API Level and Target API Level are compatible with ARCore (typically minimum API 24)
- **Check Project Validation**: In **Edit → Project Settings → XR Plug-in Management**, use **Project Validation** to identify and fix any issues

### Device Not Detected

- Verify USB debugging is enabled on your device
- Try clicking **Refresh** in the Build Profiles window
- Check USB cable connection (try a different cable)
- Install device-specific USB drivers if needed (Windows)
- Run `adb devices` in PowerShell to verify device recognition

### Build Succeeds But App Crashes

- Verify your device supports ARCore: Check [ARCore supported devices](https://developers.google.com/ar/discover/supported-devices)
- Confirm ARCore is enabled in **Edit → Project Settings → XR Plug-in Management → Android**
- Check device logs: **Window → Analysis → Android Logcat** (in Unity Editor)

### Planes Not Detected

- Ensure adequate lighting in your environment
- Point camera at textured surfaces (avoid plain white walls or floors)
- Move the device slowly to allow tracking to initialize

---

## References

- [Build your application for Android](https://docs.unity3d.com/6000.1/Documentation/Manual/android-BuildProcess.html)
- [Debug on Android devices](https://docs.unity3d.com/6000.1/Documentation/Manual/android-debugging-on-an-android-device.html)
- [ARCore supported devices](https://developers.google.com/ar/discover/supported-devices)

---

## Next Steps

After successfully building and running:
- Test the AR functionality on your device
- Experiment with placing objects using the Create menu
- Use the Options menu to explore additional features (Visualize Surfaces, AR Debug Menu, etc.)
- Consider removing example assets for a minimal setup (see Step 8 in the main plan)
