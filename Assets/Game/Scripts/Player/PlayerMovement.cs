using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float movementSpeed = 200f;

        private Vector3 _movement;
        private Rigidbody _rigidbody;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            _movement.x = Input.GetAxis("Horizontal");
            _movement.z = Input.GetAxis("Vertical");
        }

        void FixedUpdate()
        {
            if (Mathf.Abs(_movement.z) > 0)
            {
                // Don't move both directions at once, and don't start to rotate until the other directions stops
                if (Mathf.Abs(_movement.x) > 0)
                {
                    BlockRotation();
                }

                // Rotate forward/backward
                _rigidbody.constraints |= RigidbodyConstraints.FreezePositionX;
                _rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionZ;

                _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationZ;
                _rigidbody.constraints &= ~RigidbodyConstraints.FreezeRotationX;

                _rigidbody.AddTorque(new Vector3(_movement.z, 0, 0) * movementSpeed, ForceMode.VelocityChange);
            }
            else if (Mathf.Abs(_movement.x) > 0)
            {
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

        void BlockRotation()
        {
            _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationZ;
            _rigidbody.constraints |= RigidbodyConstraints.FreezeRotationX;
        }
    }
}
