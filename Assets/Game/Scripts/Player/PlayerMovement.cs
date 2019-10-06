using System;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float movementSpeed = 10f;
        [Range(0.01f, 45f)] public float rotationDegreesToAllowDirectionSwitch = 10f;
        public float slerpRotationNormalizationIdle = .25f;
        public float maxJumpVelocity = .2f;

        private Vector3 _movement;
        private Rigidbody _rigidbody;
        private bool _horizontalMovement;
        private int _lastMovementDegrees;
        private ParticleSystem _movementParticles;

        public int LastMovementDegrees
        {
            get => _lastMovementDegrees;
            private set
            {
                if (_lastMovementDegrees == value)
                {
                    return;
                }

                _lastMovementDegrees = value;
                var shape = _movementParticles.shape;
                shape.rotation = new Vector3(0, 180 + _lastMovementDegrees, 0);
            }
        }

        void Awake()
        {
            _movementParticles = GameObject.Find("MovementParticles").GetComponent<ParticleSystem>()
                                 ?? throw new Exception();
            _rigidbody = GetComponent<Rigidbody>() ?? throw new Exception();
        }

        void Update()
        {
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.z = Input.GetAxisRaw("Vertical");

            // Enable or disable particle emission based on input
            var particleEmission = _movementParticles.emission;
            particleEmission.enabled = _movement.x != 0 ^ _movement.z != 0;

            if (Input.GetKey(KeyCode.C))
            {
                GameObject.Find("Main Camera").GetComponent<FollowPlayer>().ZoomIn(
                    p => p, // Follow player's rotation (alternatively just copy it once)
                    Vector3.zero,
                    () => transform.Find("Light").GetComponent<LightScript>().ChargeLight(5)
                );
            }
        }

        void FixedUpdate()
        {
            if (Mathf.Abs(_movement.z) > 0)
            {
                // Don't move both directions at once, and don't start to rotate until the other directions stops
                if (Mathf.Abs(_movement.x) > 0)
                {
                    BlockRotation();
                    return;
                }

                // If the previous movement was horizontal, make sure the cube stands on a side
                if (_horizontalMovement && !IsNormalized())
                {
                    BlockRotation();
                    return;
                }

                _horizontalMovement = false;

                // Rotate forward/backward
                _rigidbody.constraints |= RigidbodyConstraints.FreezePositionX;
                _rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;

                _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationZ;
                _rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationX;

                _rigidbody.AddTorque(new Vector3(_movement.z, 0, 0) * movementSpeed, ForceMode.VelocityChange);

                LastMovementDegrees = _movement.z >= 0 ? 0 : 180;
            }
            else if (Mathf.Abs(_movement.x) > 0)
            {
                // If the previous movement was vertical, make sure the cube stands on a side
                if (!_horizontalMovement && !IsNormalized())
                {
                    BlockRotation();
                    return;
                }

                _horizontalMovement = true;

                // Rotate left/right
                _rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionX;
                _rigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;

                _rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
                _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationX;

                _rigidbody.AddTorque(new Vector3(0, 0, -_movement.x) * movementSpeed, ForceMode.VelocityChange);

                LastMovementDegrees = _movement.x >= 0 ? 90 : 270;
            }
            else
            {
                BlockRotation();
            }

            // Make sure we don't jump too high
            var velocity = _rigidbody.velocity;
            if (velocity.y > maxJumpVelocity)
            {
                _rigidbody.velocity = new Vector3(velocity.x, 0, velocity.z);
            }
        }

        /// <summary>
        /// Finds out the difference in degrees before the cube is normalized
        /// </summary>
        /// <param name="horizontal">horizontal difference if true, vertical difference if false</param>
        /// <returns>rotation difference in degrees</returns>
        float RotationDifferenceBeforeNormalized(bool horizontal)
        {
            var difference = horizontal
                ? transform.rotation.eulerAngles.x % 90
                : transform.rotation.eulerAngles.z % 90;

            while (difference > 45)
            {
                difference -= 90;
            }

            return Mathf.Abs(difference);
        }

        bool IsNormalized()
        {
            return Mathf.Max(
                       RotationDifferenceBeforeNormalized(true), RotationDifferenceBeforeNormalized(false)
                   ) <= rotationDegreesToAllowDirectionSwitch;
        }

        /// <summary>
        /// Normalizes the cube to stand on a side.
        /// </summary>
        void BlockRotation()
        {
            _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationZ;
            _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationX;

            // Interpolate towards a normalized cube standing up to a required degree of precision
            var rotation = transform.rotation;
            var slerpTarget = Quaternion.Slerp(
                rotation,
                Quaternion.Euler(
                    Mathf.Round(rotation.eulerAngles.x / 90f) * 90,
                    Mathf.Round(rotation.eulerAngles.y / 90f) * 90,
                    Mathf.Round(rotation.eulerAngles.z / 90f) * 90
                ),
                slerpRotationNormalizationIdle
            );
            _rigidbody.MoveRotation(slerpTarget);

            // Also prevent a redundant cube movement, but let it fall down - also make sure it's not jumping up
            var velocity = _rigidbody.velocity;
            _rigidbody.velocity = new Vector3(0, velocity.y < 0 ? velocity.y : 0, 0);
        }
    }
}
