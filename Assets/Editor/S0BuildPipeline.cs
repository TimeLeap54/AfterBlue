using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace AfterBlue.EditorTools
{
    public static class S0BuildPipeline
    {
        private const string OutputPath = "Builds/S0_Baseline/Windows/AfterBlue.exe";

        public static void BuildWindowsDevelopment()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));

            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/FishingScene.unity" },
                locationPathName = OutputPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.Development | BuildOptions.AllowDebugging
            };

            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new System.Exception($"S0 development build failed: {report.summary.result}");
            }
        }
    }
}
