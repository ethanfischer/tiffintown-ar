#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PlayModeScreenshotMenu
{
    internal const string KeyPending   = "PMS_Pending";
    internal const string KeyPath      = "PMS_Path";
    internal const string KeySuperSize = "PMS_SuperSize";

    [MenuItem("Tools/Capture Screenshot (Auto)")]
    public static void CaptureScreenshotInPlayMode()
    {
        // <ProjectRoot>/Screenshots/Screenshot_YYYYMMDD_HHMMSS.png
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string folder = Path.Combine(projectRoot, "Screenshots");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        string fileName = $"Screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        string fullPath = Path.Combine(folder, fileName);

        EditorPrefs.SetBool(KeyPending, true);
        EditorPrefs.SetString(KeyPath, fullPath);
        EditorPrefs.SetInt(KeySuperSize, 1);

        Debug.Log($"[PlayModeScreenshot] Will save to: {fullPath}");

        if (!EditorApplication.isPlaying)
            EditorApplication.isPlaying = true;
        else
            Debug.Log("[PlayModeScreenshot] Already in Play Mode; will capture this session.");
    }
}
#endif
