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

        public void SetActiveGameWon(bool active)
        {
            transform.Find("GameWon").gameObject.SetActive(active);
        }
    }
}
