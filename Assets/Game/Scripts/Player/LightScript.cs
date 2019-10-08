using System;
using System.Collections;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(Light))]
    public class LightScript : MonoBehaviour
    {
        [Range(0.05f, 10f)] public float startIntensity = 0;
        [Range(0.05f, 50f)] public float maximumIntensity = 5;
        [Range(0.05f, 10f)] public float intensityOfUpgrade = 5;
        public float lightDec = 0.005f;
        [Range(0.05f, 5f)] public float breathingIntensity = 0.2f;
        [Range(0f, 60f)] public float breathingIntervalSec = 5f;
        public BatteryUi batteryUi;

        private Light _light;

        public float LightIntensity { get; private set; }

        private void Awake()
        {
            _light = GetComponent<Light>() ?? throw new Exception();
        }

        void Start()
        {
            _light.shadows = LightShadows.Soft;
            ChargeLight(startIntensity);
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
            ChargeLight(LightIntensity - lightDec, true);
        }

        public void doDamage(float dmg)
        {
            ChargeLight(LightIntensity - dmg, true);
        }

        public void ChargeLight(float newLightIntensity, bool allowDecrease = false)
        {
            if (newLightIntensity < 0)
            {
                newLightIntensity = 0;
            }

            if (!allowDecrease && LightIntensity > newLightIntensity)
            {
                return;
            }

            LightIntensity = newLightIntensity;
            _light.intensity = LightIntensity;
            if (batteryUi != null)
            {
                batteryUi.ChangePercentage(
                    (LightIntensity - breathingIntensity) / (maximumIntensity - breathingIntensity) * 100);
            }
        }

        public void UpgradeBattery()
        {
            maximumIntensity += intensityOfUpgrade;
            ChargeLight(LightIntensity + intensityOfUpgrade);
        }
    }
}
