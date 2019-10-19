using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace Game.Scripts.UI
{
    public class MobileJoystickAutoHide : MonoBehaviour
    {
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        private void Update()
        {
            if (CrossPlatformInputManager.s_HardwareInput.GetAxis("Horizontal", true) != 0
                || CrossPlatformInputManager.s_HardwareInput.GetAxis("Vertical", true) != 0)
            {
                // Keyboard always disables touch input permanently
                gameObject.SetActive(false);
            }
            else if (CrossPlatformInputManager.s_TouchInput.GetAxis("Horizontal", true) != 0
                     || CrossPlatformInputManager.s_TouchInput.GetAxis("Vertical", true) != 0)
            {
                // Touch input only removes the arrows hint
                transform.Find("Arrows").GetComponent<Image>().enabled = false;
            }
        }
    }
}
