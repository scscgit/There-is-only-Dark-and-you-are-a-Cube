using System.Collections;
using System.Collections.Generic;
using Game.Scripts.UI;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    [Range(1f, 10f)] public float maximumIntensity = 5;
    public float lightDec = 0.005f;
    [Range(0.1f, .9f)] public float breathingIntensity = 0.2f;
    [Range(0f, 60f)] public float breathingIntervalSec = 5f;
    public new Light light;
    public BatteryUi batteryUi;

    private float _lightIntensity;

    void Start()
    {
        light.shadows = LightShadows.Soft;
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
        light.transform.position = transform.position;
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
        light.intensity = _lightIntensity;
        if (batteryUi != null)
        {
            batteryUi.ChangePercentage(
                (_lightIntensity - breathingIntensity) / (maximumIntensity - breathingIntensity) * 100);
        }
    }
}
