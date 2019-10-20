using System.Collections.Generic;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class Inventory : MonoBehaviour
    {
        public List<DoorKey> keyList = new List<DoorKey>();

        public bool HasDoorKey()
        {
            return keyList.Count > 0;
        }

        public bool UseDoorKey()
        {
            if (HasDoorKey())
            {
                keyList.RemoveAt(0);
                KeyUi.Instance.UpdateKeys(keyList);
                return true;
            }

            return false;
        }

        public void AddDoorKey()
        {
            keyList.Add(new DoorKey());
            KeyUi.Instance.UpdateKeys(keyList);
        }
    }
}
