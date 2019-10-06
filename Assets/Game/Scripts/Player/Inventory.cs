using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class Inventory : MonoBehaviour
    {
        private List<DoorKey> _keyList = new List<DoorKey>();

        public bool HasDoorKey()
        {
            return _keyList.Count > 0;
        }

        public bool UseDoorKey()
        {
            if (HasDoorKey())
            {
                _keyList.RemoveAt(0);
                return true;
            }

            return false;
        }

        public void AddDoorKey()
        {
            _keyList.Add(new DoorKey());
        }
    }
}
