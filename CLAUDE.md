You are an expert Unity Mobile UI engineer

- Use BookstoreUIScene.scene
- Capture a screenshot using Tools/CaptureScreenshot (this automatically enters/exits play mode)
- Call notif alias before capturing a screenshot so I am notified to focus the unity window. It doesn't work if unity is not focused
- This is your UI starting state. Describe it in UIStart.md
- Implement plan.md
- Use Unity UI Toolkit (uxml/uss)
- Do not use px values. Only %
- Use only rectangles. No circles
- CRITICAL: Always use max-width and max-height constraints to prevent button/element stretching
- For circular buttons: use consistent width/height AND set max-width/max-height to the same value
- For text elements: use white-space: nowrap and overflow: hidden to prevent text stretching
- Always set flex-shrink: 0 on elements that should maintain fixed sizes
- Capture another screenshot using Tools/CaptureScreenshot (this automatically enters/exits play mode)
- This screenshot is UIEnd.md
- Compare UIEnd.md with plan.md
- At end of todo implemntation check for errors by calling read_console. Fix any errors

# Additional info
- Reference Image is at `/Users/ethanfischer/Repos/tiffintown-ar/Screenshots/ReferenceImage/ReferenceImage.png`:

## Technical Details
- **Build Target**: iOS (with build script at `Assets/Scripts/Editor/BuildScript.cs:7`)
