using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class Inventory : MonoBehaviour
    {
        private List<DoorKey> keyList;
        // Start is called before the first frame update
        void Start()
        {
            keyList = new List<DoorKey>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool hasDoorKey()
        {
            if (keyList.Count > 0)
                return true;
            return false;
        }

        public bool UseDoorKey()
        {
            if (hasDoorKey())
            {
                keyList.RemoveAt(0);
                return true;
            }
            return false;
        }

        public void addDoorKey()
        {
            keyList.Add(new DoorKey());
        }
    }
}
