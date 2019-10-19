using System;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class BatteryUi : MonoBehaviour
    {
        private RectTransform _rect;
        private Image _image;
        private RectTransform _fullMask;
        private Transform _full;
        private LightScript _playerLight;
        private float _savedBattery;
        private float _lastPercentage;
        private float _lastMaxIntensity;

        void Awake()
        {
            _rect = GetComponent<RectTransform>() ?? throw new Exception();
            _image = GetComponent<Image>() ?? throw new Exception();
            _playerLight = GameObject.Find("Player/Light").GetComponent<LightScript>() ?? throw new Exception();
            _fullMask = transform.Find("FullMask").GetComponent<RectTransform>() ?? throw new Exception();
            _full = transform.Find("Full") ?? throw new Exception();
            _full.transform.SetParent(_fullMask.transform, true);
        }

        public void ChangePercentage(float newPercentage)
        {
            // Change the battery size based on maximum intensity
            SetSize();

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

        private void SetSize()
        {
            var size = _playerLight.maximumIntensity;
            // FPS optimization to save effort if there's no change to be done
            if (_lastMaxIntensity == size)
            {
                return;
            }

            // Hide the UI for very small max intensity values
            if (size < 1)
            {
                _image.enabled = false;
                // Also hide the colorful image behind mask in case player uses charger (value: max minus breathing)
                _full.gameObject.SetActive(false);
            }
            else if (_lastMaxIntensity < 1)
            {
                _image.enabled = true;
                _full.gameObject.SetActive(true);
            }

            _lastMaxIntensity = size;

            // Update the size of Battery UI based on the current max. intensity, including the mask of a full battery
            size = 4 + size / 3.5f;
            _full.transform.SetParent(transform, true);
            _rect.anchorMin = new Vector2(
                0.11f,
                0.89f - 0.01f * size
            );
            _rect.anchorMax = new Vector2(
                0.11f + 0.01f * size,
                0.89f
            );
            // The sizeDelta (pixel difference added on top of anchors) is approximately of the same magnitude,
            // so that we gain double effect - one based on responsive display ratio, and the second one using pixels
            _rect.sizeDelta = new Vector2(10f * size, 5f * size);
            _full.transform.SetParent(_fullMask.transform, true);
        }

        private void DisplayPercentage(float percentage)
        {
            // FPS optimization to only resize if there's a large enough, visible difference
            if (Mathf.Abs(_fullMask.anchorMin.x - (1 - percentage / 100f)) < 0.005f)
            {
                return;
            }

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

        private void OnMouseDown()
        {
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
