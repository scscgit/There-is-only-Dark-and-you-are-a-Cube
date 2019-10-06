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

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name);
            switch (other.gameObject.name)
            {
                case "BatteryMesh":
                    transform.Find("Light").GetComponent<LightScript>().ChargeLight(5);
                    //Destroy(col.gameObject);
                    break;
                default:
                    break;
            }
        }

        void OnCollisionEnter(Collision col)
        {
            switch (col.gameObject.name)
            {
                case "DoorEntrance":
                    Destroy(col.gameObject);
                    break;
                case "DoorKey":
                    this.GetComponent<Inventory>().addDoorKey();
                    Destroy(col.gameObject);
                    break;
                case "KeyDoorEntrance":
                    if (this.GetComponent<Inventory>().UseDoorKey())
                        Destroy(col.gameObject);
                    break;
                default:
                    break;
            }
        }
    }
}
