using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class FadeInOut : MonoBehaviour
    {
        [Range(0.001f, 0.1f)] public float speed = 0.05f;

        private Image _black;
        private float _current;
        private Action _action;

        void Awake()
        {
            _black = transform.Find("Black").GetComponent<Image>();
        }

        private void FixedUpdate()
        {
            _action?.Invoke();
        }

        public void FadeOutAndIn(Action between = null, Action after = null)
        {
            FadeOut(() =>
            {
                between?.Invoke();
                FadeIn(after);
            });
        }

        public void FadeIn(Action after = null)
        {
            SetAlpha(1);
            _current = 1;
            _action = () =>
            {
                _current -= speed;
                if (_current <= 0)
                {
                    _current = 0;
                    _action = after;
                }

                SetAlpha(_current);
            };
        }

        public void FadeOut(Action after = null)
        {
            SetAlpha(0);

            _current = 0;
            _action = () =>
            {
                _current += speed;
                if (_current >= 1)
                {
                    _current = 1;
                    _action = after;
                }

                SetAlpha(_current);
            };
        }

        void SetAlpha(float alpha)
        {
            var tint = _black.color;
            tint.a = alpha;
            _black.color = tint;
        }
    }
}
