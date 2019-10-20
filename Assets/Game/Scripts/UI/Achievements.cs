using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Game.Scripts.UI
{
    public class Achievements : MonoBehaviour
    {
        [Serializable]
        public class Achievement
        {
            public static readonly Achievement FirstBattery =
                new Achievement("first_battery", "Let there be light!");

            public static readonly Achievement WalkOnFirstTurret =
                new Achievement("walk_on_first_turret", "Are you still there? Target lost");

            public static readonly Achievement TurretHitSlidingEnemy =
                new Achievement("turret_hit_sliding_enemy", "Friendly fire");

            public static readonly Achievement SimpleWin =
                new Achievement("simple_win", "Easy mode winner");

            public static readonly Achievement NoHitWin =
                new Achievement("no_hit_win", "No-Hit Sans");

            public string Id { get; }
            public string Text { get; }

            public Achievement(string id, string text)
            {
                Id = id;
                Text = text;
            }
        }

        public static Achievements Instance { get; private set; }

        [Range(0.5f, 8f)] public float displayTime = 2f;
        [Range(0f, 1f)] public float displayLerp = 0.1f;
        [Range(0f, 1f)] public float hideLerp = 0.2f;
        [Range(0, 4f)] public float breakTime = .8f;
        public List<Achievement> displayQueue;

        public List<string> gainedAchievements;

        // Formerly Dictionary<string, object> achievementConditions, which couldn't be serialized
        public List<string> disqualifiedAchievements;

        private CanvasGroup _image;
        private Text _text;

        public void GetHit()
        {
            if (!disqualifiedAchievements.Contains(Achievement.NoHitWin.Id))
            {
                disqualifiedAchievements.Add(Achievement.NoHitWin.Id);
            }
        }

        public bool Win()
        {
            var receivedHit = disqualifiedAchievements.Contains(Achievement.NoHitWin.Id);
            Gain(receivedHit
                ? Achievement.SimpleWin
                : Achievement.NoHitWin);
            return receivedHit;
        }

        public void Gain(Achievement achievement)
        {
            if (gainedAchievements.Contains(achievement.Id))
            {
                return;
            }

            gainedAchievements.Add(achievement.Id);
            displayQueue.Add(achievement);
            GameAnalytics.Instance.Achievement(achievement);

            // Only start the display consumption sequence if there is none running
            // (which is deterministic based on the queue count, as coroutines run on the main thread)
            if (displayQueue.Count == 1)
            {
                StartCoroutine(DisplayCoroutine());
            }
        }

        private void Awake()
        {
            _image = transform.Find("Image").GetComponent<CanvasGroup>() ?? throw new Exception();
            _text = transform.Find("Image/Text").GetComponent<Text>() ?? throw new Exception();
            _image.alpha = 0;
        }

        private void OnEnable()
        {
            if (Instance != null)
            {
                throw new Exception();
            }

            Instance = this;
        }

        private IEnumerator DisplayCoroutine()
        {
            while (displayQueue.Count > 0)
            {
                yield return Display(displayQueue[0]);
                yield return new WaitForSeconds(displayTime);
                yield return Hide();
                yield return new WaitForSeconds(breakTime);
                displayQueue.RemoveAt(0);
            }
        }

        private IEnumerator Display(Achievement achievement)
        {
            _text.text = achievement.Text;
            while (_image.alpha < 0.99f)
            {
                _image.alpha = Mathf.Lerp(_image.alpha, 1, displayLerp);
                yield return null;
            }
        }

        private IEnumerator Hide()
        {
            while (_image.alpha > 0.01f)
            {
                _image.alpha = Mathf.Lerp(_image.alpha, 0, hideLerp);
                yield return null;
            }
        }
    }
}
