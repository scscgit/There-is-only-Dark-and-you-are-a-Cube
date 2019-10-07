using System;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class BatteryUi : MonoBehaviour
    {
        private RectTransform _rect;
        private RectTransform _fullMask;
        private Transform _full;
        private LightScript _playerLight;
        private float _savedBattery;
        private float _lastPercentage;

        void Awake()
        {
            _rect = GetComponent<RectTransform>() ?? throw new Exception();
            _playerLight = GameObject.Find("Player/Light").GetComponent<LightScript>() ?? throw new Exception();
            _fullMask = transform.Find("FullMask").GetComponent<RectTransform>() ?? throw new Exception();
            _full = transform.Find("Full") ?? throw new Exception();
            _full.transform.SetParent(_fullMask.transform, true);
        }

        public void ChangePercentage(float newPercentage)
        {
            // Change the battery size based on maximum intensity
            SetSize(_playerLight.maximumIntensity);

            if (newPercentage > 100)
            {
                throw new Exception($"Invalid percentage range, received value {newPercentage}");
            }

            // TODO: this works, but it could be simplified to prevent redundant calls :)
            if (_savedBattery > 0)
            {
                if (newPercentage > 0)
                {
                    // Attempt to save the battery if the user has chosen to save it
                    StoreBatteryCharge(Mathf.Max(_savedBattery, _playerLight.LightIntensity));
                }

                _lastPercentage = Mathf.Max(_lastPercentage, newPercentage);
            }
            else
            {
                _lastPercentage = newPercentage;
            }

            DisplayPercentage(_lastPercentage);
        }

        private void SetSize(float size)
        {
            _full.transform.SetParent(transform, true);
            _rect.anchorMin = new Vector2(0.1f, 1f - 0.01f * size);
            _rect.anchorMax = new Vector2(0.01f * size, 0.9f);
            _full.transform.SetParent(_fullMask.transform, true);
        }

        private void DisplayPercentage(float percentage)
        {
            // We need to modify the mask while it's not the parent, so it won't resize the image. There's no other way
            _full.transform.SetParent(transform, true);
            var mask = _fullMask.anchorMin;
            mask.x = 1 - percentage / 100f;
            _fullMask.anchorMin = mask;
            _full.transform.SetParent(_fullMask.transform, true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleBatteryStorage();
            }
        }

        [Obsolete]
        private void OnMouseDown()
        {
            // Doesn't work, and I don't know why :(
            ToggleBatteryStorage();
        }

        private void ToggleBatteryStorage()
        {
            // Save or apply the battery charge
            if (_savedBattery > 0)
            {
                var charge = _savedBattery;
                _savedBattery = 0;
                _playerLight.ChargeLight(charge);
            }
            else
            {
                StoreBatteryCharge(_playerLight.LightIntensity);
            }
        }

        private void StoreBatteryCharge(float saveBattery)
        {
            _savedBattery = saveBattery;
            _playerLight.ChargeLight(0, true);
        }
    }
}
