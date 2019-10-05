using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    private float lightIntensity { get; set; }
    public float lightDec = 0.005f;
    private Light lightComp;
    private GameObject lightGameObject;

    // Start is called before the first frame update
    void Start()
    {
        lightIntensity = 5;

        // Make a game object
        lightGameObject = new GameObject("The Light");

        // Add the light component
        lightComp = lightGameObject.AddComponent<Light>();

        // Set color and position
        lightComp.type = LightType.Point;
        lightComp.color = Color.yellow;
        lightComp.intensity = lightIntensity;

        // Set the position (or any transform property)
        followPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (lightIntensity > 0)
            lightIntensity -= lightDec;
        lightComp.intensity = lightIntensity;
        followPlayer();
    }

    private void followPlayer()
    {
        lightGameObject.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    }

    public void chargeLight(float newLightIntensity)
    {
        lightIntensity = newLightIntensity;
    }
}
