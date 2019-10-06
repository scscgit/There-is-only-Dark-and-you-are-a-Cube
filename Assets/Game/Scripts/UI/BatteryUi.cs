using System;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class BatteryUi : MonoBehaviour
    {
        private RectTransform _fullMask;
        private Transform _full;

        void Awake()
        {
            _fullMask = transform.Find("FullMask").GetComponent<RectTransform>();
            _full = transform.Find("Full");
            _full.transform.SetParent(_fullMask.transform, true);
        }

        public void ChangePercentage(float newPercentage)
        {
            if (newPercentage > 100)
            {
                throw new Exception($"Invalid percentage range, received value {newPercentage}");
            }

            _full.transform.SetParent(transform, true);
            var mask = _fullMask.anchorMin;
            mask.x = 1 - newPercentage / 100f;
            _fullMask.anchorMin = mask;
            _full.transform.SetParent(_fullMask.transform, true);
        }
    }
}
