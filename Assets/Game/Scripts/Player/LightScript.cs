using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Inspector;
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
        [ReadOnlyWhenPlaying] public Material[] controlEmissions;

        private Light _light;
        private Color[] _controlEmissionsOriginalColors;

        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        public float LightIntensity { get; private set; }

        private void Awake()
        {
            _light = GetComponent<Light>() ?? throw new Exception();
            _controlEmissionsOriginalColors = new Color[controlEmissions.Length];
            for (var i = 0; i < controlEmissions.Length; i++)
            {
                _controlEmissionsOriginalColors[i] = controlEmissions[i].GetColor(EmissionColor);
            }
        }

        private void OnDestroy()
        {
            // Revert editor changes caused during game
            for (var i = 0; i < controlEmissions.Length; i++)
            {
                controlEmissions[i].SetColor(EmissionColor, _controlEmissionsOriginalColors[i]);
            }
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

        public void DoDamage(float damage)
        {
            ChargeLight(LightIntensity - damage, true);
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
            var percentage = (LightIntensity - breathingIntensity) / (maximumIntensity - breathingIntensity);
            if (batteryUi != null)
            {
                batteryUi.ChangePercentage(percentage * 100);
            }

            // Update emissions as a percentage
            for (var i = 0; i < controlEmissions.Length; i++)
            {
                controlEmissions[i].SetColor(
                    EmissionColor,
                    _controlEmissionsOriginalColors[i] * percentage
                );
            }
        }

        public void UpgradeBattery()
        {
            maximumIntensity += intensityOfUpgrade;
            ChargeLight(LightIntensity + intensityOfUpgrade);
        }
    }
}
