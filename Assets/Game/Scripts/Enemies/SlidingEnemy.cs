using System;
using System.Collections.Generic;
using System.Linq;
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
        private Vector3 _startingPosition;

        private readonly Vector3 LeftRayStart = new Vector3(-0.35f, 0, -0.35f);
        private readonly Vector3 RightRayStart = new Vector3(0.35f, 0, 0.35f);

        private void OnDrawGizmos()
        {
            // Beam to the sky if there isn't any Player
            var playerPosition =
                GameObject.Find("Player")?.transform.position ?? transform.position + Vector3.up * playerProximity;
            var start = transform.position + transform.rotation * LeftRayStart;
            Gizmos.color = (playerPosition - start).magnitude < playerProximity ? Color.red : Color.green;
            Gizmos.DrawRay(start, (playerPosition - start).normalized * playerProximity);
            start = transform.position + transform.rotation * RightRayStart;
            Gizmos.color = (playerPosition - start).magnitude < playerProximity ? Color.red : Color.green;
            Gizmos.DrawRay(start, (playerPosition - start).normalized * playerProximity);
        }

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>() ?? throw new Exception();
            _animator = GetComponent<Animator>() ?? throw new Exception();
            _playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>() ?? throw new Exception();
            _startingPosition = transform.position;
        }

        void FixedUpdate()
        {
            // Prevent a glitch by making them stack and walking over them, getting on the wall :)
            if (transform.position.y > _startingPosition.y)
            {
                var position = transform.position;
                position.y = _startingPosition.y;
                transform.position = position;
            }

            var distanceToPlayer = _playerMovement.transform.position - transform.position;
            if (PlayerProximity(distanceToPlayer))
            {
                if (!_followingPlayer)
                {
                    _followingPlayer = true;
                    _animator.Play("SlidingStart");
                }

                StepTowards(distanceToPlayer);
            }
            else
            {
                if (_followingPlayer)
                {
                    _followingPlayer = false;
                    _animator.Play("SlidingEnd");
                }

                // Go back to spawn
                var distanceToStart = _startingPosition - transform.position;
                if (distanceToStart.magnitude > 1)
                {
                    StepTowards(distanceToStart);
                }
            }
        }

        private void StepTowards(Vector3 distanceToTarget)
        {
            var slerp = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(distanceToTarget, Vector3.up),
                rotationSpeed
            );
            var moveRotation = transform.rotation.eulerAngles;
            moveRotation.y = slerp.eulerAngles.y;
            _rigidbody.MoveRotation(Quaternion.Euler(moveRotation));
            _rigidbody.velocity = distanceToTarget.normalized * speed;
        }

        bool PlayerProximity(Vector3 distanceToPlayer)
        {
            if (!onlyDirectPath)
            {
                return Mathf.Abs(distanceToPlayer.magnitude) < playerProximity;
            }

            // Uses two rays to find a player
            return PlayerFirstHitRay(Physics.RaycastAll(
                       transform.position + transform.rotation * LeftRayStart,
                       distanceToPlayer,
                       playerProximity
                   ).OrderBy(hit => hit.distance))
                   && PlayerFirstHitRay(Physics.RaycastAll(
                       transform.position + transform.rotation * RightRayStart,
                       distanceToPlayer,
                       playerProximity
                   ).OrderBy(hit => hit.distance));
        }

        bool PlayerFirstHitRay(IEnumerable<RaycastHit> hits)
        {
            foreach (var hit in hits)
            {
                var hitTransform = hit.collider.gameObject.transform;
                if (hitTransform.IsChildOf(transform))
                {
                    // Ignore own children
                    continue;
                }

                // Visualize the impact
                //Debug.DrawRay(hit.point, Vector3.up * 5);

                // Only the First collider matters
                // Alternatively hitTransform.IsChildOf(_playerMovement.transform)
                return hit.collider.gameObject.tag.Equals("Player");
            }

            return false;
        }
    }
}
