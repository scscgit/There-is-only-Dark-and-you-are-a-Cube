using UnityEngine;

namespace Game.Scripts
{
    public static class Helper
    {
        public static void DeleteSafely(this GameObject gameObject)
        {
            gameObject.SetActive(false);
            Object.Destroy(gameObject);
        }
    }
}
