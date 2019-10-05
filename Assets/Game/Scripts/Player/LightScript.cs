using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    public float lightDec = 0.005f;
    public new Light light;

    private float _lightIntensity = 5;

    void Start()
    {
        light.intensity = _lightIntensity;
        light.shadows = LightShadows.Soft;

        // Set the position (or any transform property)
        FollowPlayer();
    }

    void Update()
    {
        if (_lightIntensity > 0)
        {
            _lightIntensity -= lightDec;
        }

        light.intensity = _lightIntensity;
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        light.transform.position = transform.position;
    }

    public void ChargeLight(float newLightIntensity)
    {
        _lightIntensity = newLightIntensity;
    }
}
