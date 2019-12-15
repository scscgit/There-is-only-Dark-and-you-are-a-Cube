using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Editor
{
    /// <summary>
    /// Courtesy of https://medium.com/@lynxelia/unity-memory-management-how-to-manage-memory-and-reduce-the-time-it-takes-to-enter-play-mode-fd07b43c1daa
    /// </summary>
    [InitializeOnLoad]
    public static class MemoryManager
    {
        static MemoryManager()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            GarbageCollect();
        }

        [MenuItem("Tools/Garbage Collect")]
        static void GarbageCollect()
        {
            EditorUtility.UnloadUnusedAssetsImmediate();
            GC.Collect();
        }
    }
}
