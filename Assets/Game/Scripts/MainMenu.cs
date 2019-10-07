using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartGame();
            }
        }

        public void StartGame()
        {
            try
            {
                SceneManager.LoadScene(1);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                try
                {
                    SceneManager.LoadScene("SimpleScene");
                }
                catch (Exception e2)
                {
                    Debug.LogException(e2);
                    SceneManager.LoadScene(AssetBundle.LoadFromFile("Assets/Game/Scene").GetAllScenePaths()[0]);
                }
            }
        }
    }
}
