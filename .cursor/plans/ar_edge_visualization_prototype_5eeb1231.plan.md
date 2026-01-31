---
name: AR Edge Visualization Prototype
overview: Create a prototype AR app that visualizes detected plane edges as 3D wireframe lines. The system will listen to ARPlane boundary changes, extract boundary vertices, and render them as LineRenderer objects in AR space.
todos: []
---

# AR Edge Visualization Prototype

## Overview

Enhance the existing AR Mobile template app to visualize detected plane edges as 3D wireframe lines. This demonstrates AR functionality by showing the boundaries of detected surfaces as AR objects.

## Current State

The app already has:

- AR plane detection via `ARPlaneManager` in `XR Origin (AR Rig)`
- Plane visualization system in [`ar-example/Assets/MobileARTemplateAssets/Scripts/ARTemplateMenuManager.cs`](ar-example/Assets/MobileARTemplateAssets/Scripts/ARTemplateMenuManager.cs)
- Plane tracking via `OnPlaneChanged` callback

## Implementation Plan

### 1. Create Edge Visualizer Script

Create [`ar-example/Assets/MobileARTemplateAssets/Scripts/ARPlaneEdgeVisualizer.cs`](ar-example/Assets/MobileARTemplateAssets/Scripts/ARPlaneEdgeVisualizer.cs):

- Component that attaches to each detected ARPlane GameObject
- Subscribes to `ARPlane.boundaryChanged` event
- Extracts boundary vertices using `ARPlane.GetBoundary()` or `ARPlane.boundary` property
- Creates/updates `LineRenderer` component to draw wireframe connecting boundary points
- Configures LineRenderer with visible material (e.g., bright color, appropriate width)

### 2. Integrate with Existing Plane Manager

Modify [`ar-example/Assets/MobileARTemplateAssets/Scripts/ARTemplateMenuManager.cs`](ar-example/Assets/MobileARTemplateAssets/Scripts/ARTemplateMenuManager.cs):

- In `OnPlaneChanged` method, add edge visualizer component to new planes
- Track edge visualizers in a dictionary similar to existing plane visualizers
- Clean up edge visualizers when planes are removed
- Add toggle option to show/hide edges (can reuse existing plane visibility toggle or add separate control)

### 3. Create Edge Material

Create a material for the edge lines:

- Bright, visible color (e.g., cyan or yellow) for contrast
- Unlit shader for consistent visibility
- Configure in Unity Editor

### 4. Configure LineRenderer Settings

- Line width: ~0.01-0.02 units (visible but not too thick)
- Use world space coordinates
- Loop the line to close the boundary
- Position at plane height (or slightly above to avoid z-fighting)

### 5. Testing Considerations

- Test with XR Simulation in Editor first
- Verify edges update when planes expand/merge
- Ensure performance is acceptable with multiple planes
- Test on Android device after Editor validation

## Technical Details

**ARPlane Boundary Access:**

- Use `ARPlane.boundary` (NativeArray<Vector2>) for boundary points in plane-local space
- Transform to world space using plane's transform
- Or use `ARPlane.GetBoundary()` if available in AR Foundation version

**LineRenderer Setup:**

- Set `useWorldSpace = true`
- Set `loop = true` to close the boundary
- Use `SetPositions()` to update vertices efficiently
- Material should be unlit for consistent AR visibility

## Files to Create/Modify

1. **New:** `ar-example/Assets/MobileARTemplateAssets/Scripts/ARPlaneEdgeVisualizer.cs` - Edge visualization component
2. **Modify:** `ar-example/Assets/MobileARTemplateAssets/Scripts/ARTemplateMenuManager.cs` - Integrate edge visualizers
3. **New:** Material asset for edge lines (created in Unity Editor)

## Success Criteria

- When planes are detected, their edges are visible as wireframe lines
- Edges update dynamically as planes expand or merge
- Edges are cleaned up when planes are removed
- Performance remains acceptable with multiple detected planes
- Works in both Editor (XR Simulation) and on Android device