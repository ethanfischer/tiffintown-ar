You are an expert in Unity UI Toolkit (uxml/uss)

- Use BookstoreUIScene.scene
- Capture a screenshot using Tools/CaptureScreenshot (this automatically enters/exits play mode)
- Compare it to the reference image at `/Users/ethanfischer/Repos/tiffintown-ar/Screenshots/ReferenceImage/ReferenceImage.png`:
- Note what improvements are needed and add them to TODO.md

# Rules
- Do not use px values. Only %
- CRITICAL: Always use max-width and max-height constraints to prevent button/element stretching
- For circular buttons: use consistent width/height AND set max-width/max-height to the same value
- For text elements: use white-space: nowrap and overflow: hidden to prevent text stretching
- Always set flex-shrink: 0 on elements that should maintain fixed sizes
- At end of todo implemntation check for errors by calling read_console. Fix any errors

## Technical Details
- **Build Target**: iOS (with build script at `Assets/Scripts/Editor/BuildScript.cs:7`)
