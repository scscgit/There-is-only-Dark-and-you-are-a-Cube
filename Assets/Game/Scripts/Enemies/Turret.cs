using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Enemies
{
    public class Turret : MonoBehaviour
    {
        public GameObject projectile;
        [Range(0.1f, 60f)] public float rotationSpeed = 4f;
        [Range(2, 100)] public int startShootRate = 20;

        private int _shootRate;
        private List<GameObject> _bullets;
        private AudioSource[] _shotSound;
        private int _shotSoundIndex;

        // Start is called before the first frame update
        void Start()
        {
            _shotSound = GetComponents<AudioSource>();
            _bullets = new List<GameObject>();
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
                bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 30, ForceMode.VelocityChange);
                _bullets.Add(bullet);
                if (_bullets.Count >= 30)
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
            if (_bullets.Count > 0)
            {
                GameObject tempOb = _bullets[0];
                _bullets.RemoveAt(0);
                tempOb.DeleteSafely();
            }
        }
    }
}
