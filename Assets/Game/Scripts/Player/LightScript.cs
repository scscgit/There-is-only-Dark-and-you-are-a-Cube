using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    private float lightIntensity { get; set; }
    public float lightDec;
    private Light lightComp;

    // Start is called before the first frame update
    void Start()
    {

        // Make a game object
        GameObject lightGameObject = new GameObject("The Light");

        // Add the light component
        lightComp = lightGameObject.AddComponent<Light>();

        // Set color and position
        lightComp.color = Color.white;
        lightComp.intensity = lightIntensity/10;

        // Set the position (or any transform property)
        lightGameObject.transform.position = new Vector3(0, 5, 0);
        lightIntensity = 100;
        lightDec = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (lightIntensity>0)
            lightIntensity -= lightDec;
        lightComp.intensity = lightIntensity/10;
    }
}
