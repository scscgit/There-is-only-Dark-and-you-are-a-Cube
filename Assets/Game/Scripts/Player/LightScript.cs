using System.Collections;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(Light))]
    public class LightScript : MonoBehaviour
    {
        [Range(0.05f, 10f)] public float maximumIntensity = 5;
        public float lightDec = 0.005f;
        [Range(0.05f, .9f)] public float breathingIntensity = 0.2f;
        [Range(0f, 60f)] public float breathingIntervalSec = 5f;
        public BatteryUi batteryUi;

        private float _lightIntensity;
        private Light _light;

        private void Awake()
        {
            _light = GetComponent<Light>();
        }

        void Start()
        {
            _light.shadows = LightShadows.Soft;
            ChargeLight(maximumIntensity);

            // Set the position (or any transform property)
            FollowPlayer();
        }

        private void OnEnable()
        {
            StartCoroutine(BreathingImpulse());
        }

        private IEnumerator BreathingImpulse()
        {
            while (breathingIntervalSec > 0)
            {
                yield return new WaitForSeconds(breathingIntervalSec);
                ChargeLight(breathingIntensity);
            }
        }

        void FixedUpdate()
        {
            ChargeLight(_lightIntensity - lightDec, true);

            FollowPlayer();
        }

        private void FollowPlayer()
        {
            _light.transform.position = transform.position;
        }

        public void ChargeLight(float newLightIntensity, bool allowDecrease = false)
        {
            if (newLightIntensity < 0)
            {
                newLightIntensity = 0;
            }

            if (!allowDecrease && _lightIntensity > newLightIntensity)
            {
                return;
            }

            _lightIntensity = newLightIntensity;
            _light.intensity = _lightIntensity;
            if (batteryUi != null)
            {
                batteryUi.ChangePercentage(
                    (_lightIntensity - breathingIntensity) / (maximumIntensity - breathingIntensity) * 100);
            }
        }
    }
}
