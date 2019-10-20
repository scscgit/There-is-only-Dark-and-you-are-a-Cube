using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.UI;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

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
            _light = transform.Find("Light").GetComponent<LightScript>() ?? throw new Exception();
        }

        private void Start()
        {
            Analytics.FlushEvents();
            GameAnalytics.Instance.LevelStart();
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

                    var checkpointManager = GameObject.Find("Player").GetComponent<CheckpointManager>();
                    var checkpoint = other.gameObject.transform.parent.gameObject;
                    var alreadySaved = checkpointManager.AddCheckpoint(checkpoint);

                    GameAnalytics.Instance.UseCharger(
                        alreadySaved,
                        checkpointManager.checkpoints,
                        checkpoint.transform.position
                    );

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
                                stopZoom1 =>
                                {
                                    followPlayer.zoomSpeed *= 1.8f;
                                    followPlayer.ZoomIn(
                                        p => 0,
                                        followPlayer.zoomOffset,
                                        stopZoom2 => fadeInOut.FadeOutAndIn(() =>
                                        {
                                            followPlayer.zoomSpeed /= 1.8f;
                                            HUD.Instance.SetActiveCheckpointGained(false);
                                            stopZoom2();
                                            _zooming = false;
                                        })
                                    );
                                },
                                true
                            );
                        });
                    break;

                case "AboveTurret":
                    Achievements.Instance.Gain(Achievements.Achievement.WalkOnFirstTurret);
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
            if (!col.gameObject.activeSelf)
            {
                // A very important check to make sure no duplicate collisions occur!
                return;
            }

            switch (col.gameObject.tag)
            {
                case "Battery":
                    GameAnalytics.Instance.UpgradeBattery();
                    Achievements.Instance.Gain(Achievements.Achievement.FirstBattery);
                    _light.UpgradeBattery();
                    col.gameObject.DeleteSafely();
                    break;
                case "Key":
                    GetComponent<Inventory>().AddDoorKey();
                    GameAnalytics.Instance.CollectKey();
                    col.gameObject.DeleteSafely();
                    break;
                case "Enemy":
                    if (_light.LightIntensity > 0)
                    {
                        Achievements.Instance.GetHit();
                        GameAnalytics.Instance.HitByEnemy();
                    }

                    _light.DoDamage(10f);
                    break;
            }

            switch (col.gameObject.name)
            {
                case "Bullet(Clone)":
                    if (col.gameObject.GetComponent<Bullet>().TryFirstHit())
                    {
                        Achievements.Instance.GetHit();
                        var velocityMagnitude = col.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
                        GameAnalytics.Instance.HitByBullet(velocityMagnitude);
                        _light.DoDamage(velocityMagnitude);
                    }

                    break;
                case "KeyEntrance":
                    if (GetComponent<Inventory>().UseDoorKey())
                    {
                        GameAnalytics.Instance.UsedKeyOnDoor();
                        col.gameObject.DeleteSafely();
                    }

                    break;
                case "WinEntrance":
                    if (_gameWon)
                    {
                        return;
                    }

                    var receivedHit = Achievements.Instance.Win();
                    GameAnalytics.Instance.GameOver(GetComponent<Inventory>().keyList);
                    Analytics.FlushEvents();

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
                                    HUD.Instance.SetGameWon(receivedHit);
                                });
                        },
                        true
                    );
                    break;
            }
        }
    }
}
