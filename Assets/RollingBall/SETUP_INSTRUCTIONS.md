# RollingBall VR Setup Instructions

## Overview
This guide will help you set up the RollingBall game in VR within the Parkour project. The VR infrastructure is already configured in the Parkour project, so you just need to set up the scene.

## What's Been Done
✅ Copied all RollingBall assets (Prefabs, Scripts, Materials) to `Parkour/Assets/RollingBall/`
✅ Created `RollingBallVR.unity` scene in `Parkour/Assets/Scenes/`
✅ Created `PlayerControllerVR.cs` script with VR controller support

## Unity Setup Steps (1h30 is plenty of time!)

### Step 1: Open the Scene (2 minutes)
1. Open Unity and load the **Parkour** project
2. Navigate to `Assets/Scenes/RollingBallVR.unity`
3. Open the scene

### Step 2: Add VR Camera Rig (10 minutes)
1. In the Hierarchy, **DELETE** the existing "Main Camera"
2. Right-click in Hierarchy → **XR** → **XR Origin (Action-based)** or find the OVRCameraRig prefab
   - If using Meta Quest: Look for `OVRCameraRig` prefab in the Oculus folder
3. This will add the VR camera and controllers to your scene
4. Position the XR Origin at a good starting point (e.g., `Y = 2` so you're above the ground)

### Step 3: Update the Player Object (10 minutes)
1. Find the **Player** object in the Hierarchy
2. In the Inspector, **remove** the old `PlayerController` component
3. **Add Component** → Search for `PlayerControllerVR` → Add it
4. Assign the references in the Inspector:
   - **Text Count**: Drag the UI Text object that shows the coin count
   - **Win Text**: Drag the UI Text object that shows win/lose message
   - **Win Object**: Drag the panel/canvas that displays the win/lose screen
   - **Camera Transform**: Drag the VR Camera (usually under XR Origin → Camera Offset → Center Eye Anchor or Main Camera)
5. Adjust settings if needed:
   - **Max Speed**: 50 (already set)
   - **Acceleration Speed**: 5 (already set)

### Step 4: Fix UI for VR (15 minutes)
The UI needs to be world-space for VR:
1. Find the **Canvas** in the Hierarchy
2. In the Inspector, change **Render Mode** to **World Space**
3. Position the canvas in front of the player:
   - Position: `(0, 3, 5)` (5 units in front, 3 units up)
   - Scale: `(0.01, 0.01, 0.01)` (make it smaller)
   - Rotation: `(0, 180, 0)` (face the player)
4. You may need to adjust the text size to make it readable

### Step 5: Test Basic Movement (5 minutes)
1. **Save the scene** (Ctrl+S / Cmd+S)
2. Connect your VR headset
3. Click **Play** in Unity
4. Test controls:
   - **Left Thumbstick**: Move the ball
   - **A Button**: Restart after game over

### Step 6: Optional Enhancements (30+ minutes)
If you have time, add these improvements:

#### 6.1 Direct Selection with Raycasting
1. Create a new script `CoinPickupVR.cs` to pick up coins by pointing at them
2. Add ray visualization from controllers
3. Use trigger button to grab coins at a distance

#### 6.2 Hand Presence
1. Add hand models to the controllers
2. Use OVR Hand prefabs for hand tracking (optional)

#### 6.3 Better Feedback
1. Add haptic feedback when collecting coins:
```csharp
OVRInput.SetControllerVibration(0.5f, 0.5f, OVRInput.Controller.LTouch);
```
2. Add spatial audio to coins

## VR Controls
- **Left Thumbstick**: Move the ball (relative to where you're looking)
- **A Button** (after game over): Restart the scene
- **Look around**: Move your head to look around in VR

## Troubleshooting

### Ball doesn't move
- Check that `PlayerControllerVR` is attached to the Player object
- Verify Camera Transform is assigned in the Inspector
- Make sure the VR headset is connected and tracked

### Can't see UI
- UI must be in World Space mode for VR
- Position it in front of the player
- Make sure it's not too close or too far

### VR not working
- Verify `OVRCameraRig` or `XR Origin` is in the scene
- Check Project Settings → XR Plug-in Management → Oculus/OpenXR is enabled
- Make sure your headset is connected before pressing Play

## For Your Blog Post
Document:
1. **Setup process**: Screenshots of Unity hierarchy and Inspector settings
2. **Code changes**: Explain the differences between `PlayerController` and `PlayerControllerVR`
3. **VR implementation**: How thumbstick input translates to ball movement
4. **Challenges**: What issues did you encounter and how did you solve them?
5. **Video**: Record gameplay footage from the headset showing the VR experience

## Next Steps for Homework
According to Lab Homework 4, you need to:
1. ✅ Implement the roll-a-ball example (done - copied from RollingBall project)
2. ✅ Adapt to VR version (done - created PlayerControllerVR)
3. ⏸️ Implement direct selection and raycasting (optional enhancement above)

Good luck! You should be able to get basic VR functionality working in under an hour, leaving time for testing and optional features.
