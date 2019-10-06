using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerCollisionChecker : MonoBehaviour
    {
        private bool _zooming;

        private void OnTriggerEnter(Collider other)
        {
            switch (other.gameObject.name)
            {
                case "BatteryMesh":
                    if (_zooming)
                    {
                        return;
                    }

                    _zooming = true;
                    var offset = new Vector3(0, 1, -2);
                    var followPlayer = GameObject.Find("Main Camera").GetComponent<FollowPlayer>();
                    followPlayer.zoomSpeed = 0.005f;
                    //float? playerRotation = null;
                    followPlayer.ZoomIn(
                        p => 0, //p => playerRotation ?? (playerRotation = p).Value,
                        offset,
                        () => followPlayer.ZoomIn(
                            p => 0, offset, () => { _zooming = false; }
                        ),
                        true
                    );
                    break;
            }
        }

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
                    GetComponent<Inventory>().AddDoorKey();
                    Destroy(col.gameObject);
                    break;
                case "KeyEntrance":
                    if (GetComponent<Inventory>().UseDoorKey())
                    {
                        Destroy(col.gameObject);
                    }

                    break;
                case "WinEntrance":
                    // TODO
                    transform.localScale = new Vector3(3, 3, 3);

                    var offset = new Vector3(0, 1, -2);
                    var followPlayer = GameObject.Find("Main Camera").GetComponent<FollowPlayer>();
                    followPlayer.zoomSpeed = 0.005f;
                    followPlayer.ZoomIn(
                        p => 0,
                        offset,
                        () => { },
                        true
                    );
                    break;
            }
        }
    }
}
