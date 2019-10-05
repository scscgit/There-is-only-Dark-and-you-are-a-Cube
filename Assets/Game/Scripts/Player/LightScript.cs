using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    private float lightIntensity { get; set; }
    public float lightDec = 0.005f;
    public Light light;

    // Start is called before the first frame update
    void Start()
    {
        lightIntensity = 5;

        // Set color and position
        light.type = LightType.Point;
        light.color = Color.yellow;
        light.intensity = lightIntensity;

        // Set the position (or any transform property)
        followPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (lightIntensity>0)
            lightIntensity -= lightDec;
        light.intensity = lightIntensity;
        followPlayer();
    }

    private void followPlayer()
    {
        light.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    }

    public void chargeLight(float newLightIntensity)
    {
        lightIntensity = newLightIntensity;
    }
}
