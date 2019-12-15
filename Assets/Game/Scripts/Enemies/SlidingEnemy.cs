using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Player;
using Game.Scripts.UI;
using UnityEngine;

namespace Game.Scripts.Enemies
{
    [SelectionBase]
    public class SlidingEnemy : MonoBehaviour
    {
        public bool alive = true;
        public float playerProximity = 5f;
        public bool onlyDirectPath = true;
        public float speed = 1.5f;
        public float rotationSpeed = .05f;

        private Rigidbody _rigidbody;
        private Animator _animator;
        private PlayerMovement _playerMovement;
        private bool _followingPlayer;
        private Vector3 _startingPosition;

        private readonly Vector3 LeftRayStart = new Vector3(-0.35f, 0, -0.35f);
        private readonly Vector3 RightRayStart = new Vector3(0.35f, 0, 0.35f);
        private static readonly int Sliding = Animator.StringToHash("sliding");

        // Physics movement
        private Quaternion? _moveRotation;
        private Vector3 _moveVelocity;

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

        void Update()
        {
            if (!alive)
            {
                return;
            }

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
                    _animator.SetBool(Sliding, true);
                }
                else
                {
                    // Only move towards player if this is the second iteration (marked by true _followingPlayer),
                    // otherwise the movement would glitch back and forth with Player in places like a door
                    StepTowards(distanceToPlayer);
                }
            }
            else
            {
                if (_followingPlayer)
                {
                    _followingPlayer = false;
                    _animator.SetBool(Sliding, false);
                }

                // Go back to spawn
                var distanceToStart = _startingPosition - transform.position;
                if (distanceToStart.magnitude > 0.1)
                {
                    StepTowards(distanceToStart);
                }
                // Don't go anywhere
                else
                {
                    _moveVelocity = Vector3.zero;
                }
            }
        }

        private void FixedUpdate()
        {
            if (_moveRotation.HasValue)
            {
                _rigidbody.MoveRotation(_moveRotation.Value);
                _moveRotation = null;
            }

            // Only move if there's no vertical rebound (this is a hotfix, the Y position is overridden anyway)
            if (Mathf.Abs(_rigidbody.velocity.y) < 0.01)
            {
                _rigidbody.velocity = _moveVelocity;
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
            _moveRotation = Quaternion.Euler(moveRotation);
            _moveVelocity = distanceToTarget.normalized * speed;
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

        void OnCollisionEnter(Collision col)
        {
            switch (col.gameObject.name)
            {
                case "Bullet(Clone)":
                    // A bullet can only hit once, and it has to be powerful enough
                    if (col.gameObject.GetComponent<Bullet>().TryFirstHit()
                        && col.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 2)
                    {
                        Achievements.Instance.Gain(Achievements.Achievement.TurretHitSlidingEnemy);
                        _animator.enabled = false;
                        alive = false;
                    }

                    // Include bullet as a rebound
                    goto case "Player";

                // Rebound
                case "Player":
                case "SlidingEnemy":
                    _rigidbody.velocity =
                        Vector3.Reflect(_rigidbody.velocity.normalized * 5f, col.contacts[0].normal)
                        // Vertical velocity only acts as a timer
                        + Vector3.up * 0.5f;

                    break;
            }
        }
    }
}
