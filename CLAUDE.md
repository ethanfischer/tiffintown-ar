You are an expert in Unity UI Toolkit (uxml/uss)

You can capture a screenshot using Tools/CaptureScreenshot (this automatically enters/exits play mode)
Screenshots are stored at `/Users/ethanfischer/Repos/tiffintown-ar/Screenshots/`
Reference image is at `/Users/ethanfischer/Repos/tiffintown-ar/Screenshots/ReferenceImage/ReferenceImage.png`:

# Rules
- Do not use px values. Only %
- CRITICAL: Always use max-width and max-height constraints to prevent button/element stretching
- For circular buttons: use consistent width/height AND set max-width/max-height to the same value
- For text elements: use white-space: nowrap and overflow: hidden to prevent text stretching
- Always set flex-shrink: 0 on elements that should maintain fixed sizes
- NEVER use bare numbers in CSS - always include units (px, %, etc.) for border-width, margin, padding, etc.

# Common Issues & Solutions
- **Child elements appearing outside parent**: Usually caused by conflicting CSS sizing (e.g., width: 80% + max-width: 40px). Use ONLY max-width/max-height for fixed-size elements
- **Fragmented UI elements**: Check UXML structure - related elements should be children of same container for unified styling
- **Layout breaking on resize**: Add max-width to root container to prevent horizontal stretching beyond mobile dimensions
