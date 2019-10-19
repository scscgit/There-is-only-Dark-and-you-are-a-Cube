using System.Linq;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput.PlatformSpecific
{
    public class MultiplexorInput : VirtualInput
    {
        private VirtualInput[] _inputs;
        private VirtualInput _configureVirtualInput;

        /// <summary>
        /// Self-explanatory. NOTE: only the first input is used for MousePosition purposes.
        /// </summary>
        public MultiplexorInput(VirtualInput[] inputs, VirtualInput configureVirtualInput)
        {
            _inputs = inputs;
            _configureVirtualInput = configureVirtualInput;
        }

        public override bool AxisExists(string name)
        {
            return _configureVirtualInput.AxisExists(name);
        }

        public override bool ButtonExists(string name)
        {
            return _configureVirtualInput.ButtonExists(name);
        }

        public override void RegisterVirtualAxis(CrossPlatformInputManager.VirtualAxis axis)
        {
            _configureVirtualInput.RegisterVirtualAxis(axis);
        }

        public override void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
        {
            _configureVirtualInput.RegisterVirtualButton(button);
        }

        public override void UnRegisterVirtualAxis(string name)
        {
            _configureVirtualInput.UnRegisterVirtualAxis(name);
        }

        public override void UnRegisterVirtualButton(string name)
        {
            _configureVirtualInput.UnRegisterVirtualButton(name);
        }

        public override CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
        {
            return _configureVirtualInput.VirtualAxisReference(name);
        }

        public override void SetVirtualMousePositionX(float f)
        {
            _configureVirtualInput.SetVirtualMousePositionX(f);
        }

        public override void SetVirtualMousePositionY(float f)
        {
            _configureVirtualInput.SetVirtualMousePositionY(f);
        }

        public override void SetVirtualMousePositionZ(float f)
        {
            _configureVirtualInput.SetVirtualMousePositionZ(f);
        }

        public override float GetAxis(string name, bool raw)
        {
            var max = _inputs.Select(v => Mathf.Abs(v.GetAxis(name, raw))).Max();
            return Mathf.Clamp(_inputs.Select(v => v.GetAxis(name, raw)).Sum(), -max, max);
        }

        public override bool GetButton(string name)
        {
            return _inputs.Select(v => v.GetButton(name)).Any();
        }

        public override bool GetButtonDown(string name)
        {
            return _inputs.Select(v => v.GetButtonDown(name)).Any();
        }

        public override bool GetButtonUp(string name)
        {
            return _inputs.Select(v => v.GetButtonUp(name)).Any();
        }

        public override void SetButtonDown(string name)
        {
            _configureVirtualInput.SetButtonDown(name);
        }

        public override void SetButtonUp(string name)
        {
            _configureVirtualInput.SetButtonUp(name);
        }

        public override void SetAxisPositive(string name)
        {
            _configureVirtualInput.SetAxisPositive(name);
        }

        public override void SetAxisNegative(string name)
        {
            _configureVirtualInput.SetAxisNegative(name);
        }

        public override void SetAxisZero(string name)
        {
            _configureVirtualInput.SetAxisZero(name);
        }

        public override void SetAxis(string name, float value)
        {
            _configureVirtualInput.SetAxis(name, value);
        }

        public override Vector3 MousePosition()
        {
            return _inputs.First().MousePosition();
        }
    }
}
