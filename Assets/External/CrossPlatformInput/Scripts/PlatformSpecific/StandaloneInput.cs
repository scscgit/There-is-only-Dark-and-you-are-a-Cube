using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput.PlatformSpecific
{
    public class StandaloneInput : VirtualInput
    {
        public override bool AxisExists(string name)
        {
            throw NotPossible();
        }

        public override bool ButtonExists(string name)
        {
            throw NotPossible();
        }

        public override void RegisterVirtualAxis(CrossPlatformInputManager.VirtualAxis axis)
        {
            throw NotPossible();
        }

        public override void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
        {
            throw NotPossible();
        }

        public override void UnRegisterVirtualAxis(string name)
        {
            throw NotPossible();
        }

        public override void UnRegisterVirtualButton(string name)
        {
            throw NotPossible();
        }

        public override CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
        {
            throw NotPossible();
        }

        public override void SetVirtualMousePositionX(float f)
        {
            throw NotPossible();
        }

        public override void SetVirtualMousePositionY(float f)
        {
            throw NotPossible();
        }

        public override void SetVirtualMousePositionZ(float f)
        {
            throw NotPossible();
        }

        public override float GetAxis(string name, bool raw)
        {
            return raw ? Input.GetAxisRaw(name) : Input.GetAxis(name);
        }

        public override bool GetButton(string name)
        {
            return Input.GetButton(name);
        }

        public override bool GetButtonDown(string name)
        {
            return Input.GetButtonDown(name);
        }

        public override bool GetButtonUp(string name)
        {
            return Input.GetButtonUp(name);
        }

        public override void SetButtonDown(string name)
        {
            throw NotPossible();
        }

        public override void SetButtonUp(string name)
        {
            throw NotPossible();
        }


        public override void SetAxisPositive(string name)
        {
            throw NotPossible();
        }

        public override void SetAxisNegative(string name)
        {
            throw NotPossible();
        }

        public override void SetAxisZero(string name)
        {
            throw NotPossible();
        }

        public override void SetAxis(string name, float value)
        {
            throw NotPossible();
        }

        private Exception NotPossible()
        {
            return new Exception(
                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }

        public override Vector3 MousePosition()
        {
            return Input.mousePosition;
        }
    }
}
