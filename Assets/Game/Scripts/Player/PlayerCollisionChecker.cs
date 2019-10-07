using Game.Scripts.UI;
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
                case "ChargerMesh":
                    if (_zooming)
                    {
                        return;
                    }

                    _zooming = true;
                    var followPlayer = GameObject.Find("Main Camera").GetComponent<FollowPlayer>();
                    var fadeInOut = GameObject.Find("FadeInOut").GetComponent<FadeInOut>();
                    //float? playerRotation = null;
                    fadeInOut.FadeOutAndIn(
                        () => followPlayer.ZoomIn(
                            p => 0, //p => playerRotation ?? (playerRotation = p).Value,
                            followPlayer.zoomOffset,
                            stopZoom1 => followPlayer.ZoomIn(
                                p => 0,
                                followPlayer.zoomOffset,
                                stopZoom2 => fadeInOut.FadeOutAndIn(() =>
                                {
                                    stopZoom2();
                                    _zooming = false;
                                })
                            ),
                            true
                        )
                    );
                    break;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            switch (other.gameObject.name)
            {
                case "ChargerMesh":
                    transform.Find("Light").GetComponent<LightScript>().ChargeLight(5);
                    break;
            }
        }

        void OnCollisionEnter(Collision col)
        {
            switch (col.gameObject.tag)
            {
                case "Battery":
                    transform.Find("Light").GetComponent<LightScript>().ChargeLight(5);
                    Destroy(col.gameObject);
                    break;
                case "Key":
                    GetComponent<Inventory>().AddDoorKey();
                    Destroy(col.gameObject);
                    break;
                default:
                    break;
            }

            switch (col.gameObject.name)
            {
                case "Bullet(Clone)":
                    transform.Find("Light").GetComponent<LightScript>().doDamage(0.4f);
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
                        stopZoom =>
                        {
                            // The End
                        },
                        true
                    );
                    break;
            }
        }
    }
}
