using UnityEngine;
using UnityEditor;

namespace Game.Scripts.Editor
{
    [InitializeOnLoad]
    class CompileTime : EditorWindow
    {
        static bool IsCompilingTrackingTime;
        static bool IsPlaymodeTrackingTime;
        static bool HasPlaymodeTracked = true;
        static double EnterPlaymodeStartTime;
        static double CompileStartTime;

        static CompileTime()
        {
            EditorApplication.update += Update;
            CompileStartTime = PlayerPrefs.GetFloat("CompileStartTime", 0);
            if (CompileStartTime > 0)
            {
                IsCompilingTrackingTime = true;
            }
        }

        static void Update()
        {
            if (EditorApplication.isCompiling && !IsCompilingTrackingTime)
            {
                CompileStartTime = EditorApplication.timeSinceStartup;
                PlayerPrefs.SetFloat("CompileStartTime", (float) CompileStartTime);
                IsCompilingTrackingTime = true;
            }
            else if (!EditorApplication.isCompiling && IsCompilingTrackingTime)
            {
                var finishTime = EditorApplication.timeSinceStartup;
                IsCompilingTrackingTime = false;
                var compileTime = finishTime - CompileStartTime;
                PlayerPrefs.DeleteKey("CompileStartTime");
                Debug.Log("Script compilation time: \n" + compileTime.ToString("0.000") + "s");
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode && !IsPlaymodeTrackingTime &&
                !EditorApplication.isPlaying)
            {
                IsPlaymodeTrackingTime = true;
                EnterPlaymodeStartTime = EditorApplication.timeSinceStartup;
                PlayerPrefs.SetFloat("EnterPlaymodeStartTime", (float) EnterPlaymodeStartTime);
            }
            else if (EditorApplication.isPlaying && HasPlaymodeTracked)
            {
                HasPlaymodeTracked = false;
                var finishTime = EditorApplication.timeSinceStartup;
                var playmodeLoadTime = finishTime - PlayerPrefs.GetFloat("EnterPlaymodeStartTime");
                PlayerPrefs.DeleteKey("EnterPlaymodeStartTime");
                Debug.Log("Playmode load time: \n" + playmodeLoadTime.ToString("0.000") + "s");
            }
        }
    }
}
