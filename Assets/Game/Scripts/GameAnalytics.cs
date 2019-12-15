using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Player;
using Game.Scripts.UI;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace Game.Scripts
{
    public class GameAnalytics
    {
        private static GameAnalytics _instance;

        public static GameAnalytics Instance => _instance == null ? _instance = new GameAnalytics() : _instance;

        private PlayerMovement _player;
        private LightScript _light;

        public GameAnalytics()
        {
            _player = GameObject.Find("Player").GetComponent<PlayerMovement>() ?? throw new Exception();
            _light = _player.transform.Find("Light").GetComponent<LightScript>() ?? throw new Exception();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            _instance = null;
        }

        private void Event(
            Action<IDictionary<string, object>> analyticsEventType,
            bool playerMetrics,
            Dictionary<string, object> add = null)
        {
            if (add == null)
            {
                add = new Dictionary<string, object>();
            }

            // Parameters included in all events
            add.Add("time_elapsed", Time.timeSinceLevelLoad);

            // Parameters related to Player
            if (playerMetrics)
            {
                add.Add("position", _player.transform.position);
                add.Add("light_intensity", _light.LightIntensity);
                add.Add("max_light_intensity", _light.maximumIntensity);
            }

            analyticsEventType(add);
        }

        /// <summary>
        /// This overload uses a custom event, as the event type lambda isn't specified explicitly
        /// </summary>
        private void Event(string name, bool playerMetrics, Dictionary<string, object> add = null)
        {
            Event(eventData => Analytics.CustomEvent(name, eventData), playerMetrics, add);
        }

        public void LevelStart()
        {
            Event(
                eventData => AnalyticsEvent.LevelStart(SceneManager.GetActiveScene().name, eventData),
                false
            );
        }

        public void GameOver(List<DoorKey> keyList)
        {
            Event(
                eventData => AnalyticsEvent.GameOver(SceneManager.GetActiveScene().name, eventData),
                true,
                new Dictionary<string, object>
                {
                    {"keys_remaining", keyList.Count}
                }
            );
        }

        public void UseCharger(bool alreadySaved, List<GameObject> checkpoints, Vector3 thisCheckpoint)
        {
            Event("use_charger", true, new Dictionary<string, object>
            {
                {"already_saved", alreadySaved},
                {"number_of_checkpoints", checkpoints.Count},
                {"last_checkpoint", checkpoints.Last().transform.position},
                {"this_checkpoint", thisCheckpoint},
            });
        }

        public void UpgradeBattery()
        {
            Event("upgrade_battery", true);
        }

        public void CollectKey()
        {
            Event("collect_key", true);
        }

        public void HitByEnemy()
        {
            Event("hit_by_enemy", true);
        }

        public void HitByBullet(float velocityMagnitude)
        {
            Event("hit_by_bullet", true, new Dictionary<string, object>
            {
                {"bullet_velocity_magnitude", velocityMagnitude},
            });
        }

        public void UsedKeyOnDoor()
        {
            Event("used_key_on_door", true);
        }

        public void CheckpointRestart(List<GameObject> checkpoints, Vector3 checkpoint)
        {
            Event("checkpoint_restart_key", true, new Dictionary<string, object>
            {
                {"number_of_checkpoints", checkpoints.Count},
                {"last_checkpoint", checkpoint},
            });
        }

        public void Achievement(Achievements.Achievement achievement)
        {
            Event("achievement", true, new Dictionary<string, object>
            {
                {"achievement_id", achievement.Id},
            });
        }
    }
}
