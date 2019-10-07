using System;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts
{
    public class FollowPlayer : MonoBehaviour
    {
        public PlayerMovement player;

        public AnimationCurve zoomPitchCurve = AnimationCurve.EaseInOut(0, 0, 1, 90);

        public AnimationCurve zoomDistanceCurve = new AnimationCurve(
            new Keyframe(0, 0.5f) {outTangent = 0},
            new Keyframe(1, 60) {inTangent = 90}
        );

        public bool applyZoom;
        [Range(0.0001f, 0.1f)] public float zoomSpeed = 0.002f;
        [Range(0.01f, 1f)] public float zoomRotationSpeed = 0.1f;
        public Vector3 zoomOffset = new Vector3(0, 2f, 0);

        private Vector3 _offset;
        private float _zoom = 1;
        private bool _zoomOut;
        private float _zoomRotation;
        private Quaternion _startingRotation;
        private event Action<Action> AfterZoom;
        private Func<float, float> _zoomRotationFunc = input => input;

        void Start()
        {
            _offset = transform.position - player.transform.position;
            _startingRotation = transform.rotation;
        }

        void Update()
        {
            if (applyZoom)
            {
                _zoom += _zoomOut ? zoomSpeed : -zoomSpeed;
                if (_zoomOut && _zoom >= 1 || _zoom <= 0)
                {
                    if (AfterZoom == null)
                    {
                        StopZoom();
                    }
                    else
                    {
                        // Only run the invocation once and then do a noop
                        var invocation = AfterZoom;
                        AfterZoom = stopZoom => { };
                        invocation.Invoke(StopZoom);
                    }

                    return;
                }

                transform.rotation = Quaternion.Euler(
                    zoomPitchCurve.Evaluate(_zoom), RotateAroundTowards(_zoomRotationFunc(player.LastMovementDegrees)),
                    0.0f);
                transform.position =
                    player.transform.position + zoomOffset
                    - transform.rotation * Vector3.forward * zoomDistanceCurve.Evaluate(_zoom);
                return;
            }

            transform.position = new Vector3(
                player.transform.position.x + _offset.x, _offset.y, player.transform.position.z + _offset.z);
        }

        float RotateAroundTowards(float degrees)
        {
            _zoomRotation = Quaternion.Lerp(
                Quaternion.Euler(0, _zoomRotation, 0),
                Quaternion.Euler(0, degrees, 0),
                zoomRotationSpeed
            ).eulerAngles.y;
            return _zoomRotation;
        }

        /// <param name="afterZoom">Don't forget to invoke the action to finish the ZOOM unless you provide null</param>
        public void ZoomIn(
            Func<float, float> _zoomRotationDegreesParamPlayer,
            Vector3 offset,
            Action<Action> afterZoom = null,
            bool zoomOut = false)
        {
            _zoomOut = zoomOut;
            applyZoom = true;
            _zoom = _zoomOut ? 0 : 1;
            zoomOffset = offset;
            _zoomRotationFunc = _zoomRotationDegreesParamPlayer;
            AfterZoom = afterZoom;

            // Do a first iteration, so that the StopZoom() result can be overridden immediately
            Update();
        }

        public void StopZoom()
        {
            applyZoom = false;
            transform.rotation = _startingRotation;
        }
    }
}
