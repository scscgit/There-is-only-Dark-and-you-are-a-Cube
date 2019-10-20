using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Enemies
{
    public class Turret : MonoBehaviour
    {
        public GameObject projectile;
        [Range(0.1f, 60f)] public float rotationSpeed = 4f;
        [Range(2, 100)] public int startShootRate = 20;
        public List<GameObject> bullets;

        private int _shootRate;
        private AudioSource[] _shotSound;
        private int _shotSoundIndex;

        void Awake()
        {
            _shotSound = GetComponents<AudioSource>();
            _shootRate = startShootRate;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * 10);
        }

        private void FixedUpdate()
        {
            transform.Rotate(0, rotationSpeed, 0, Space.World);
            if (_shootRate == 0)
            {
                _shootRate = startShootRate;
                GameObject bullet =
                    Instantiate(projectile, transform.position + transform.forward * 0.65f, Quaternion.identity);
                bullet.transform.parent = transform.parent;
                bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 30, ForceMode.VelocityChange);
                bullets.Add(bullet);
                if (bullets.Count >= 30)
                {
                    RemoveFirstBullet();
                }

                _shotSound[_shotSoundIndex].Play();
                _shotSoundIndex = (_shotSoundIndex + 1) % _shotSound.Length;
            }

            _shootRate--;
        }

        private void RemoveFirstBullet()
        {
            if (bullets.Count > 0)
            {
                GameObject tempOb = bullets[0];
                bullets.RemoveAt(0);
                tempOb.DeleteSafely();
            }
        }
    }
}
