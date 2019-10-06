using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerCollisionChecker : MonoBehaviour
    {
        void Start()
        {
        }

        void Update()
        {
        }

        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.name == "Battery")
            {
                transform.Find("Light").GetComponent<LightScript>().ChargeLight(5);
                Destroy(col.gameObject);
            }

            if (col.gameObject.name == "DoorEntrance")
            {
                Destroy(col.gameObject);
            }
        }
    }
}
