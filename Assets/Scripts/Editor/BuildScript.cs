using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

public class BuildScript
{
    public static void BuildForIOS()
    {
        // Get scenes to include in build
        string[] scenes = new string[] { "Assets/Scenes/MainScene.unity" };
        
        // Build player options
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = "Build/iOS";
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.None;
        
        // Build the project
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;
        
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }
        
        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }
}