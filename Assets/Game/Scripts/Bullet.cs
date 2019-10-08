using UnityEngine;

namespace Game.Scripts
{
    public class Bullet : MonoBehaviour
    {
        private bool _alreadyHit;

        public bool TryFirstHit()
        {
            var firstHit = !_alreadyHit;
            _alreadyHit = true;
            return firstHit;
        }
    }
}
