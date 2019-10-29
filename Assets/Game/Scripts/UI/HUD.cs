using UnityEngine;

namespace Game.Scripts.UI
{
    public class HUD : MonoBehaviour
    {
        private static HUD _instance;

        public static HUD Instance => _instance ? _instance : _instance = GameObject.Find("HUD").GetComponent<HUD>();

        public void SetActiveCheckpointGained(bool active)
        {
            transform.Find("CheckpointGained").gameObject.SetActive(active);
        }

        public void SetGameWon(bool receivedHit)
        {
            transform.Find("GameWon").gameObject.SetActive(true);
            transform.Find("GameWon/Image/SimpleWinText").gameObject.SetActive(receivedHit);
            transform.Find("GameWon/Image/NoHitWinText").gameObject.SetActive(!receivedHit);
        }

        private void OnGUI()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                MainMenu.QualitySettingsGui();
            }

            // Debugging command to switch Camera Rendering Path
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.N))
            {
                var cam = GameObject.Find("Main Camera").GetComponent<Camera>();
                cam.renderingPath = cam.renderingPath == RenderingPath.DeferredShading
                    ? RenderingPath.Forward
                    : RenderingPath.DeferredShading;
            }
        }
    }
}
