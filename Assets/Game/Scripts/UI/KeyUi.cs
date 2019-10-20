using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class KeyUi : MonoBehaviour
    {
        public static KeyUi Instance { get; private set; }

        public float requiredKeyDuration = 2.2f;

        private Transform _requiredKey;
        private Transform _keys;
        private List<Transform> _keyCopies = new List<Transform>();
        private bool _requiredKeyDisplayed;

        private void Awake()
        {
            _requiredKey = transform.Find("RequiredKey") ?? throw new Exception();
            _requiredKey.gameObject.SetActive(false);
            _keys = transform.Find("Keys") ?? throw new Exception();
            _keys.GetChild(0).gameObject.SetActive(false);
            _keyCopies = new List<Transform>();
        }

        private void OnEnable()
        {
            if (Instance != null)
            {
                throw new Exception();
            }

            Instance = this;
        }

        public void ShowRequiredKey()
        {
            if (!_requiredKeyDisplayed)
            {
                _requiredKeyDisplayed = true;
                StartCoroutine(DisplayRequiredKey());
            }
        }

        private IEnumerator DisplayRequiredKey()
        {
            _requiredKey.gameObject.SetActive(true);
            yield return new WaitForSeconds(requiredKeyDuration);
            _requiredKey.gameObject.SetActive(false);
            _requiredKeyDisplayed = false;
        }

        public void UpdateKeys(List<DoorKey> keyList)
        {
            // Delete previous copies
            _keyCopies.ForEach(t => t.gameObject.DeleteSafely());
            _keyCopies = new List<Transform>();

            // Enable the first key if required
            var original = _keys.GetChild(0);
            if (keyList.Count < 2)
            {
                original.gameObject.SetActive(keyList.Count == 1);
                return;
            }

            // Two or more keys: spawn the duplicates
            original.gameObject.SetActive(true);
            for (var i = 1; i < keyList.Count; i++)
            {
                var duplicate = Instantiate(original).GetComponent<RectTransform>();
                duplicate.parent = _keys;
                duplicate.anchorMin = new Vector2(i, 0);
                duplicate.anchorMax = new Vector2(1 + i, 1);
                duplicate.offsetMin = Vector2.zero;
                duplicate.offsetMax = Vector2.zero;
                _keyCopies.Add(duplicate);
            }
        }
    }
}
