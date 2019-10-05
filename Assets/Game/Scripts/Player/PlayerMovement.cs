using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float movementSpeed = 200f;
        public float rotationDegreesToAllowDirectionSwitch = 15f;
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
                    BlockRotation();
                    return;
                }

                // If the previous movement was horizontal, make sure the cube stands on a side
                if (_horizontalMovement && BlockRotation())
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
                if (!_horizontalMovement && BlockRotation())
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
                BlockRotation();
            }
        }

        /// <summary>
        /// Normalizes the cube to stand on a side.
        /// </summary>
        /// <returns>true if there was a compensation rotation applied</returns>
        bool BlockRotation()
        {
            _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationZ;
            _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationX;

            var slerpTarget = Quaternion.Slerp(
                transform.rotation,
                Quaternion.Euler(
                    Mathf.Round(transform.rotation.eulerAngles.x / 90f) * 90,
                    Mathf.Round(transform.rotation.eulerAngles.y / 90f) * 90,
                    Mathf.Round(transform.rotation.eulerAngles.z / 90f) * 90
                ),
                slerpRotationNormalizationIdle
            );
            var differenceX = transform.rotation.eulerAngles.x % 90;
            while (differenceX > 45)
            {
                differenceX -= 90;
            }

            var differenceY = transform.rotation.eulerAngles.z % 90;
            while (differenceY > 45)
            {
                differenceY -= 90;
            }

            if (Mathf.Max(Mathf.Abs(differenceX), Mathf.Abs(differenceY)) > rotationDegreesToAllowDirectionSwitch)
            {
                _rigidbody.MoveRotation(slerpTarget);
                return true;
            }

            return false;
        }
    }
}
