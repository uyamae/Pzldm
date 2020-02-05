using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pzldm
{
    public class BuildUtil
    {
        [MenuItem("Pzldm/Build Exe")]
        public static void BuildExe()
        {
            Build(BuildTarget.StandaloneWindows64);
        }
        private static Dictionary<BuildTarget, string> ExtensionTable = new Dictionary<BuildTarget, string>()
        {
            { BuildTarget.StandaloneWindows, "exe" },
            { BuildTarget.StandaloneWindows64, "exe" },
            { BuildTarget.iOS, "ipa" },
            { BuildTarget.Android, "apk" },
        };
        private static void Build(BuildTarget target)
        {
            string[] scenes = new string[] {
                "Assets/Scenes/Boot/boot.unity",
                "Assets/Scenes/Select/select.unity",
                "Assets/Scenes/Battle/battle.unity"
            };
            string ext = ExtensionTable.ContainsKey(target) ? ExtensionTable[target] : "exe";
            string path = $"output/{target.ToString()}/Pzldm.{ext}";
            BuildOptions opts = BuildOptions.None;
            BuildPipeline.BuildPlayer(scenes, path, target, opts);
        }

        [MenuItem("Pzldm/Build WebGL")]
        public static void BuildWebGL()
        {
            string[] scenes = new string[] {
                "Assets/Scenes/Boot/boot.unity",
                "Assets/Scenes/Select/select.unity",
                "Assets/Scenes/Battle/battle.unity"
            };
            string path = $"../../docs";
            var target = BuildTarget.WebGL;
            BuildOptions opts = BuildOptions.None;
            BuildPipeline.BuildPlayer(scenes, path, target, opts);
        }
    }
}
