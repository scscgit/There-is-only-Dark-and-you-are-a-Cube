using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerCollisionChecker : MonoBehaviour
    {
        private bool _zooming;
        private LightScript _light;
        private bool _gameWon;

        public bool Zooming => _zooming;

        private void Awake()
        {
            _light = transform.Find("Light").GetComponent<LightScript>();
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.gameObject.name)
            {
                case "ChargerMesh":
                    if (_zooming)
                    {
                        return;
                    }

                    var alreadySaved = GameObject.Find("Player")
                        .GetComponent<CheckpointManager>()
                        .AddCheckpoint(other.gameObject.transform.parent.gameObject);

                    _zooming = true;
                    var followPlayer = GameObject.Find("Main Camera").GetComponent<FollowPlayer>();
                    var fadeInOut = GameObject.Find("FadeInOut").GetComponent<FadeInOut>();
                    //float? playerRotation = null;
                    fadeInOut.FadeOutAndIn(
                        () =>
                        {
                            if (!alreadySaved)
                            {
                                HUD.Instance.SetActiveCheckpointGained(true);
                            }

                            followPlayer.ZoomIn(
                                p => 0, //p => playerRotation ?? (playerRotation = p).Value,
                                followPlayer.zoomOffset,
                                stopZoom1 => followPlayer.ZoomIn(
                                    p => 0,
                                    followPlayer.zoomOffset,
                                    stopZoom2 => fadeInOut.FadeOutAndIn(() =>
                                    {
                                        HUD.Instance.SetActiveCheckpointGained(false);
                                        stopZoom2();
                                        _zooming = false;
                                    })
                                ),
                                true
                            );
                        });
                    break;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            switch (other.gameObject.name)
            {
                case "ChargerMesh":
                    _light.ChargeLight(_light.maximumIntensity);
                    break;
            }
        }

        void OnCollisionEnter(Collision col)
        {
            switch (col.gameObject.tag)
            {
                case "Battery":
                    _light.UpgradeBattery();
                    Destroy(col.gameObject);
                    break;
                case "Key":
                    GetComponent<Inventory>().AddDoorKey();
                    Destroy(col.gameObject);
                    break;
                case "Enemy":
                    _light.doDamage(10f);
                    break;
            }

            switch (col.gameObject.name)
            {
                case "Bullet(Clone)":
                    _light.doDamage(0.4f);
                    Destroy(col.gameObject);
                    break;
                case "KeyEntrance":
                    if (GetComponent<Inventory>().UseDoorKey())
                    {
                        Destroy(col.gameObject);
                    }

                    break;
                case "WinEntrance":
                    if (_gameWon)
                    {
                        return;
                    }

                    _gameWon = true;
                    transform.localScale = new Vector3(3, 3, 3);

                    var offset = new Vector3(0, 1, -2);
                    var followPlayer = GameObject.Find("Main Camera").GetComponent<FollowPlayer>();
                    followPlayer.zoomSpeed = 0.005f;
                    followPlayer.ZoomIn(
                        p => 0,
                        offset,
                        stopZoom =>
                        {
                            GameObject.Find("FadeInOut")
                                .GetComponent<FadeInOut>()
                                .FadeOut(() =>
                                {
                                    // Game over
                                    HUD.Instance.SetActiveGameWon(true);
                                });
                        },
                        true
                    );
                    break;
            }
        }
    }
}
