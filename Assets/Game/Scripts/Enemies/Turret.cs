using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private AudioSource shotSound;

    public GameObject projectile;
    public float rotationSpeed = 1f;
    private GameObject lightGameObject;
    private Light lightComp;
    public int shootRate = 20;
    private int startShootRate;
    private List<GameObject> bullets;
    // Start is called before the first frame update
    void Start()
    {
        shotSound = GetComponent<AudioSource>();
        bullets = new List<GameObject>();
        startShootRate = shootRate;
        // Make a game object
        lightGameObject = new GameObject("The Light");

        // Add the light component
        Light lightComp = lightGameObject.AddComponent<Light>();

        // Set color and position
        lightComp.color = Color.white;

        // Set the position (or any transform property)
        lightGameObject.transform.position = new Vector3(transform.position.x, transform.position.y+1, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward * 10, Color.red);
        this.transform.Rotate(0, rotationSpeed, 0, Space.World);
        if (shootRate==0)
        {
            shootRate = startShootRate;
            Vector3 vec = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            GameObject bullet = Instantiate(projectile, transform.position + transform.forward * 0.65f, Quaternion.identity) as GameObject;
            bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 15, ForceMode.VelocityChange);
            bullets.Add(bullet);
            if (bullets.Count >= 3)
                removeFirstBullet();

            shotSound.Play();
        }
        shootRate--;
    }

    private void removeFirstBullet()
    {
        if (bullets.Count>0)
        {
            GameObject tempOb = bullets[0];
            bullets.RemoveAt(0);
            Destroy(tempOb);
        }
    }

}
