using System;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Enemies
{
    [SelectionBase]
    public class SlidingEnemy : MonoBehaviour
    {
        public float playerProximity = 5f;
        public bool onlyDirectPath = true;
        public float speed = 1.5f;
        public float rotationSpeed = .1f;

        private Rigidbody _rigidbody;
        private Animator _animator;
        private PlayerMovement _playerMovement;
        private bool _followingPlayer;

        private void OnDrawGizmos()
        {
            // Beam to the sky if there isn't any Player
            var playerPosition =
                GameObject.Find("Player")?.transform.position ?? transform.position + Vector3.up * playerProximity;
            var direction = playerPosition - transform.position;
            Gizmos.color = direction.magnitude < playerProximity ? Color.red : Color.green;
            Gizmos.DrawRay(transform.position, direction.normalized * playerProximity);
        }

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>() ?? throw new Exception();
            _animator = GetComponent<Animator>() ?? throw new Exception();
            _playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>() ?? throw new Exception();
        }

        void Update()
        {
            var distanceToPlayer = _playerMovement.transform.position - transform.position;
            if (PlayerProximity(distanceToPlayer))
            {
                if (!_followingPlayer)
                {
                    _followingPlayer = true;
                    _animator.Play("SlidingStart");
                }

                var slerp = Quaternion.Slerp(transform.rotation, _playerMovement.transform.rotation, rotationSpeed);
                var moveRotation = transform.rotation.eulerAngles;
                moveRotation.y = slerp.eulerAngles.y;
                _rigidbody.MoveRotation(Quaternion.Euler(moveRotation));
                _rigidbody.velocity = distanceToPlayer.normalized * speed;
            }
            else
            {
                if (_followingPlayer)
                {
                    _followingPlayer = false;
                    _animator.Play("SlidingEnd");
                }
            }
        }

        bool PlayerProximity(Vector3 distanceToPlayer)
        {
            if (!onlyDirectPath)
            {
                return Mathf.Abs(distanceToPlayer.magnitude) < playerProximity;
            }

            var rayHits = Physics.RaycastAll(transform.position, distanceToPlayer, playerProximity);
            foreach (var hit in rayHits)
            {
                var hitTransform = hit.collider.gameObject.transform;
                if (hitTransform.IsChildOf(transform))
                {
                    // Ignore own children
                    continue;
                }

                // Only the First collider matters
                // Alternatively hitTransform.IsChildOf(_playerMovement.transform)
                return hit.collider.gameObject.tag.Equals("Player");
            }

            return false;
        }
    }
}
