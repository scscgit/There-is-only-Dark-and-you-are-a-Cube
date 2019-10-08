using Game.Scripts.Inspector;
using UnityEngine;

namespace Game.Scripts
{
    [SelectionBase]
    public class Door : MonoBehaviour
    {
        [ReadOnlyWhenPlaying] public bool requireKey;
        [ReadOnlyWhenPlaying] public bool finalDoorToWin;

        void Start()
        {
            transform.Find("KeyEntrance").gameObject.SetActive(requireKey);
            transform.Find("SafeEntrance").gameObject.SetActive(!requireKey && !finalDoorToWin);
            transform.Find("WinEntrance").gameObject.SetActive(finalDoorToWin);
        }
    }
}
