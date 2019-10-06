using System.Collections;
using System.Collections.Generic;
using Game.Scripts.UI;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    public float lightDec = 0.005f;
    public new Light light;

    private float _lightIntensity = 5;
    private BatteryUi _batteryUi;

    void Awake()
    {
        _batteryUi = GameObject.Find("BatteryUI").GetComponent<BatteryUi>();
    }

    void Start()
    {
        light.intensity = _lightIntensity;
        light.shadows = LightShadows.Soft;

        // Set the position (or any transform property)
        FollowPlayer();
    }

    void FixedUpdate()
    {
        if (_lightIntensity - lightDec > 0)
        {
            ChargeLight(_lightIntensity - lightDec);
        }

        FollowPlayer();
    }

    private void FollowPlayer()
    {
        light.transform.position = transform.position;
    }

    public void ChargeLight(float newLightIntensity)
    {
        _lightIntensity = newLightIntensity;
        light.intensity = _lightIntensity;
        _batteryUi.ChangePercentage(newLightIntensity / 5f * 100);
    }
}
