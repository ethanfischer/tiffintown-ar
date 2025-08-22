using UnityEngine;
using UnityEngine;

public class PlayModeScreenshotter : MonoBehaviour
{
    public string OutputPath;
    public int SuperSize = 1;

    // Auto-bootstrap in Play Mode, after the first scene is loaded
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void BootstrapFromPrefs()
    {
        #if UNITY_EDITOR
        // Use the same keys as the editor menu
        const string KeyPending = "PMS_Pending";
        const string KeyPath = "PMS_Path";
        const string KeySuperSize = "PMS_SuperSize";

        if (!UnityEditor.EditorPrefs.GetBool(KeyPending, false)) return;

        string path = UnityEditor.EditorPrefs.GetString(KeyPath, "");
        int super = UnityEditor.EditorPrefs.GetInt(KeySuperSize, 1);

        if (string.IsNullOrEmpty(path)) return;

        var go = new GameObject("__PlayModeScreenshotter");
        Object.DontDestroyOnLoad(go);
        var helper = go.AddComponent<PlayModeScreenshotter>();
        helper.OutputPath = path;
        helper.SuperSize = super;

        UnityEditor.EditorPrefs.SetBool(KeyPending, false);
        #endif
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(CaptureThenExit());
    }

    private System.Collections.IEnumerator CaptureThenExit()
    {
        // Wait one rendered frame
        yield return new WaitForEndOfFrame();

        // --- build a safe file path ---
        string path = OutputPath;

        // If OutputPath is a folder (no extension), append a timestamped filename
        if (string.IsNullOrEmpty(System.IO.Path.GetExtension(path)))
        {
            var dir = path.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            if (string.IsNullOrEmpty(dir)) dir = System.IO.Path.Combine(Application.dataPath, "../Screenshots");
            System.IO.Directory.CreateDirectory(dir);
            path = System.IO.Path.Combine(dir, $"Screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        }
        else
        {
            // Ensure its directory exists
            var dir = System.IO.Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) System.IO.Directory.CreateDirectory(dir);
        }

        // --- synchronous write ---
        var width = Screen.width;
        var height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        try
        {
            var bytes = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
            Debug.Log($"[PlayModeScreenshot] Saved screenshot to: {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PlayModeScreenshot] Failed to save screenshot: {e}");
        }
        finally
        {
            Destroy(tex);
        }

        // Exit Play Mode on the editor loop
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () =>
        {
            Debug.Log("[PlayModeScreenshot] Exiting Play Mode.");
            UnityEditor.EditorApplication.isPlaying = false;
        };
        #endif

        Destroy(gameObject);
    }
}
