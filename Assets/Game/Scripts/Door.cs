using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts
{
    public class Door : MonoBehaviour
    {
        public bool requireKey;
        public bool finalDoorToWin;

        void Start()
        {
            transform.Find("KeyEntrance").gameObject.SetActive(requireKey);
            transform.Find("SafeEntrance").gameObject.SetActive(!requireKey && !finalDoorToWin);
            transform.Find("WinEntrance").gameObject.SetActive(finalDoorToWin);
        }

        private void OnCollisionEnter(Collision other)
        {
            var player = other.gameObject.GetComponent<PlayerMovement>();
            // TODO
            player.transform.localScale = new Vector3(3, 3, 3);
        }
    }
}
