using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float movementSpeed = 200f;
        [Range(0.01f, 45f)] public float rotationDegreesToAllowDirectionSwitch = 10f;
        public float slerpRotationNormalizationIdle = .25f;

        private Vector3 _movement;
        private Rigidbody _rigidbody;
        private bool _horizontalMovement;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            _movement.x = Input.GetAxisRaw("Horizontal");
            _movement.z = Input.GetAxisRaw("Vertical");
        }

        void FixedUpdate()
        {
            if (Mathf.Abs(_movement.z) > 0)
            {
                // Don't move both directions at once, and don't start to rotate until the other directions stops
                if (Mathf.Abs(_movement.x) > 0)
                {
                    BlockRotation(null);
                    return;
                }

                // If the previous movement was horizontal, make sure the cube stands on a side
                if (_horizontalMovement && BlockRotation(null))
                {
                    return;
                }

                _horizontalMovement = false;

                // Rotate forward/backward
                _rigidbody.constraints |= RigidbodyConstraints.FreezePositionX;
                _rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;

                _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationZ;
                _rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationX;

                _rigidbody.AddTorque(new Vector3(_movement.z, 0, 0) * movementSpeed, ForceMode.VelocityChange);
            }
            else if (Mathf.Abs(_movement.x) > 0)
            {
                // If the previous movement was vertical, make sure the cube stands on a side
                if (!_horizontalMovement && BlockRotation(null))
                {
                    return;
                }

                _horizontalMovement = true;

                // Rotate left/right
                _rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionX;
                _rigidbody.constraints |= RigidbodyConstraints.FreezePositionZ;

                _rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationZ;
                _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationX;

                _rigidbody.AddTorque(new Vector3(0, 0, -_movement.x) * movementSpeed, ForceMode.VelocityChange);
            }
            else
            {
                BlockRotation(null);
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

        /// <summary>
        /// Normalizes the cube to stand on a side.
        /// </summary>
        /// <param name="rotationDegreesBeforeNormalized">rotation degrees before the cube is normalized</param>
        /// <returns>true if there was a compensation rotation applied</returns>
        bool BlockRotation(float? rotationDegreesBeforeNormalized)
        {
            _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationZ;
            _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationX;

            if (!rotationDegreesBeforeNormalized.HasValue)
            {
                rotationDegreesBeforeNormalized =
                    Mathf.Max(RotationDifferenceBeforeNormalized(true), RotationDifferenceBeforeNormalized(false));
            }

            if (rotationDegreesBeforeNormalized > rotationDegreesToAllowDirectionSwitch)
            {
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

                // Also prevent a redundant cube movement
                _rigidbody.velocity = Vector3.zero;
                return true;
            }

            return false;
        }
    }
}
