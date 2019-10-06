using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts
{
    [SelectionBase]
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
    }
}
