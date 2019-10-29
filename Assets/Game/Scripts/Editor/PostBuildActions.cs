using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Game.Scripts.Editor
{
    public class PostBuildActions : MonoBehaviour
    {
        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target == BuildTarget.WebGL)
            {
                // Disable WebGL mobile unsupported warning
                var path = Path.Combine(pathToBuiltProject, "Build/UnityLoader.js");
                var text = File.ReadAllText(path);
                text = text.Replace("UnityLoader.SystemInfo.mobile", "false");
                File.WriteAllText(path, text);
            }
        }
    }
}
