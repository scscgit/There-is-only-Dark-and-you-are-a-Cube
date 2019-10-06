using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerCollisionChecker : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            switch (other.gameObject.name)
            {
                case "BatteryMesh":
                    transform.Find("Light").GetComponent<LightScript>().ChargeLight(5);
                    break;
            }
        }

        void OnCollisionEnter(Collision col)
        {
            switch (col.gameObject.name)
            {
                case "Battery":
                    transform.Find("Light").GetComponent<LightScript>().ChargeLight(5);
                    Destroy(col.gameObject);
                    break;
                case "SafeEntrance":
                    Destroy(col.gameObject);
                    break;
                case "DoorKey":
                    GetComponent<Inventory>().addDoorKey();
                    Destroy(col.gameObject);
                    break;
                case "KeyEntrance":
                    if (GetComponent<Inventory>().UseDoorKey())
                    {
                        Destroy(col.gameObject);
                    }

                    break;
            }
        }
    }
}
