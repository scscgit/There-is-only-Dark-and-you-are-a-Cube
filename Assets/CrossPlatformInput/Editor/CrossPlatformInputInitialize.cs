using System.Collections.Generic;
using UnityEditor;

namespace UnityStandardAssets.CrossPlatformInput.Inspector
{
    public class CrossPlatformInitialize
    {
        // Custom compiler defines:
        //
        // MOBILE_INPUT : denotes that mobile input should be used right now!

        [MenuItem("Mobile Input/Enable")]
        private static void Enable()
        {
            SetEnabled("MOBILE_INPUT", true);
        }

        [MenuItem("Mobile Input/Enable", true)]
        private static bool EnableValidate()
        {
            var defines = GetDefinesList(buildTargetGroups[0]);
            return !defines.Contains("MOBILE_INPUT");
        }

        [MenuItem("Mobile Input/Disable", true)]
        private static bool DisableValidate()
        {
            var defines = GetDefinesList(buildTargetGroups[0]);
            return defines.Contains("MOBILE_INPUT");
        }

        [MenuItem("Mobile Input/Disable")]
        private static void Disable()
        {
            SetEnabled("MOBILE_INPUT", false);
        }

        private static BuildTargetGroup[] buildTargetGroups =
        {
            BuildTargetGroup.Standalone,
            BuildTargetGroup.Android,
            BuildTargetGroup.WebGL
        };

        private static void SetEnabled(string defineName, bool enable)
        {
            foreach (var group in buildTargetGroups)
            {
                var defines = GetDefinesList(group);
                if (enable)
                {
                    if (defines.Contains(defineName))
                    {
                        return;
                    }

                    defines.Add(defineName);
                }
                else
                {
                    if (!defines.Contains(defineName))
                    {
                        return;
                    }

                    while (defines.Contains(defineName))
                    {
                        defines.Remove(defineName);
                    }
                }

                string definesString = string.Join(";", defines.ToArray());
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definesString);
            }
        }

        private static List<string> GetDefinesList(BuildTargetGroup group)
        {
            return new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';'));
        }
    }
}
