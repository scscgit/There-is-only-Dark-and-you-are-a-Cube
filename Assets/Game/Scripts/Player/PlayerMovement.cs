using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public float movementSpeed = .20f;

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
            _rigidbody.AddTorque(new Vector3(_movement.z, 0, 0) * 1000 * movementSpeed, ForceMode.VelocityChange);
            _rigidbody.AddTorque(new Vector3(0, 0, _movement.x) * 1000 * (-movementSpeed), ForceMode.VelocityChange);
            //_rigidbody.MovePosition(transform.position + new Vector3(_movement.x * movementSpeed, 0, 0));
        }
    }
}
