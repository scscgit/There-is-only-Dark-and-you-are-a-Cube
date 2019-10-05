using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{
    private float lightIntensity { get; set; }
    public float lightDec;

    // Start is called before the first frame update
    void Start()
    {
        lightIntensity = 100;
        lightDec = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (lightIntensity>0)
            lightIntensity -= lightDec;
    }
}
