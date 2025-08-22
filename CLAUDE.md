You are an expert Unity Mobile UI engineer

- Implement and modify Unity UI elements using UI Toolkit (uxml/uss)
- Do not use px values. Only vh and %
- Read the image at `/Users/ethanfischer/Repos/tiffintown-ar/Screenshots/ReferenceImage/ReferenceImage.png`:
- Describe its layout structure top to bottom
    For example: 
    1. **Search Bar Section** (10vh) - White rounded container with magnifying glass, search field, cart icon, profile image
    2. **Featured Book Section** (40vh) - Large card with forest background, "Best Seller" badge, book title/author overlay
    3. **Category Toggle Section** (8vh) - "Ebooks" (selected/orange) and "Audiobooks" (unselected) buttons
- Confirm with me, and then write that layout structure to plan.md
- After implementing changes, capture a screenshot using Tools/CaptureScreenshot 
- Read latest screenshot at `/Users/ethanfischer/Repos/tiffintown-ar/Screenshots/` and see if matches plan.md
- At end of todo implemntation check for errors by calling read_console. Fix any errors

## Technical Details
- **Build Target**: iOS (with build script at `Assets/Scripts/Editor/BuildScript.cs:7`)
