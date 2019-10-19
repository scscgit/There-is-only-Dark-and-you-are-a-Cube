using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Editor
{
    public class FindByScale : ScriptableWizard
    {
        public Vector3 scale;

        [MenuItem("Tools/Select by Scale")]
        public static void DoMagic()
        {
            DisplayWizard<FindByScale>(typeof(FindByScale).Name, "Find objects w/ same scale");
        }

        private void OnWizardCreate()
        {
            Selection.objects = FindObjectsOfType<Transform>()
                .Where(t => t.localScale == scale)
                .Select(t => t.gameObject)
                .ToArray();
        }
    }
}
