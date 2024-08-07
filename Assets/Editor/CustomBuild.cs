using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class CustomBuild : MonoBehaviour
{
    [MenuItem("UIA01/Build Project")]
    public static void BuildProject()
    {
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
        string buildPath = GetBuildPath(buildTarget);
        string configFileName = GetConfigFileName(buildTarget);
        string configFilePath = Path.Combine("Assets/Configs", configFileName);

        try
        {
            EnsureDirectoryExists(Path.GetDirectoryName(buildPath)); // Ensure parent directory exists
            CopyConfigFile(configFilePath, Path.GetDirectoryName(buildPath), configFileName);
            PerformBuild(buildPath);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Build failed: {ex.Message}");
        }
    }

    private static string GetBuildPath(BuildTarget buildTarget)
    {
        string basePath = "Builds";
        switch (buildTarget)
        {
            case BuildTarget.WebGL:
                return Path.Combine(basePath, "WebGL", "index.html"); // WebGL builds output an HTML file
            case BuildTarget.iOS:
                return Path.Combine(basePath, "iOS"); // iOS builds output to a directory
            case BuildTarget.Android:
                return Path.Combine(basePath, "Android", "UIA01.apk"); // Android builds output an APK file
            case BuildTarget.StandaloneWindows:
                return Path.Combine(basePath, "Standalone", "UIA01.exe"); // Standalone builds output an executable file
            case BuildTarget.StandaloneWindows64:
                return Path.Combine(basePath, "Standalone64", "UIA01.exe"); // Standalone64 builds output an executable file
            default:
                throw new NotSupportedException($"Unsupported build target: {buildTarget}");
        }
    }

    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private static void CopyConfigFile(string sourceFilePath, string destinationDirectory, string fileName)
    {
        if (File.Exists(sourceFilePath))
        {
            string destinationFilePath = Path.Combine(destinationDirectory, fileName);
            File.Copy(sourceFilePath, destinationFilePath, true);
            Debug.Log($"Copied config file to {destinationFilePath}.");
        }
        else
        {
            throw new FileNotFoundException($"Config file {fileName} not found at {sourceFilePath}");
        }
    }

    private static void PerformBuild(string buildPath)
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetScenePaths(),
            locationPathName = buildPath,
            target = EditorUserBuildSettings.activeBuildTarget,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build completed successfully for target {EditorUserBuildSettings.activeBuildTarget} at {buildPath}.");
        }
        else
        {
            Debug.LogError($"Build failed for target {EditorUserBuildSettings.activeBuildTarget}. Errors: {summary.totalErrors}");
        }
    }
    private static string[] GetScenePaths()
    {
        // Get all scenes from the build settings
        return Array.ConvertAll(EditorBuildSettings.scenes, scene => scene.path);
    }
    private static string GetConfigFileName(BuildTarget buildTarget)
    {
        return buildTarget switch
        {
            BuildTarget.WebGL => "webgl-config.json",
            BuildTarget.iOS => "ios-config.json",
            BuildTarget.Android => "android-config.json",
            BuildTarget.StandaloneWindows or BuildTarget.StandaloneWindows64 or BuildTarget.StandaloneOSX => "standalone-config.json",
            _ => throw new NotSupportedException($"Unsupported build target: {buildTarget}")
        };
    }
}
