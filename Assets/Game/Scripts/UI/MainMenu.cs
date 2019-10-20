using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace Game.Scripts.UI
{
    public class MainMenu : MonoBehaviour
    {
        private void OnGUI()
        {
            QualitySettingsGui();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartGame();
            }
        }

        public static void QualitySettingsGui()
        {
            // Experimental quality settings
            var names = QualitySettings.names;
            if (names[0] != "High")
            {
                Debug.LogWarning("You've changed the Default Quality, please update this script!");
            }

            GUILayout.BeginVertical();
            for (var i = 1; i < names.Length; i++)
            {
                if (names[i] == "Very High")
                {
                    if (GUILayout.Button(names[0]))
                    {
                        QualitySettings.SetQualityLevel(0, true);
                    }
                }

                if (GUILayout.Button(names[i]))
                {
                    QualitySettings.SetQualityLevel(i, true);
                }
            }

            GUILayout.EndVertical();
        }

        public void StartGame()
        {
            AnalyticsEvent.GameStart(new Dictionary<string, object> {{"time_elapsed", Time.timeSinceLevelLoad}});
            try
            {
                SceneManager.LoadScene("Game/Scenes/SampleScene");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                SceneManager.LoadScene(1);
            }
        }
    }
}
