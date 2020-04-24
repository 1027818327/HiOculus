using UnityEngine;
using UnityEngine.EventSystems;

namespace Oculus
{
    public class CustomOVRInputModule : OVRInputModule
    {
        //public bool isActiveRight;

        protected override PointerEventData.FramePressState GetGazeButtonState()
        {
            bool pressed = false;
            bool released = false;

            if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
            {
                pressed = Input.GetKeyDown(gazeClickKey) || (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.RawButton.LThumbstick));
                released = Input.GetKeyUp(gazeClickKey) || (!OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && !OVRInput.Get(OVRInput.RawButton.LThumbstick));
            }
            else 
            {
                pressed = Input.GetKeyDown(gazeClickKey) || (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && OVRInput.Get(OVRInput.RawButton.RThumbstick));
                released = Input.GetKeyUp(gazeClickKey) || (!OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && !OVRInput.Get(OVRInput.RawButton.RThumbstick));
            }
            
#if UNITY_ANDROID && !UNITY_EDITOR
            // On Gear VR the mouse button events correspond to touch pad events. We only use these as gaze pointer clicks
            // on Gear VR because on PC the mouse clicks are used for actual mouse pointer interactions.
            pressed |= Input.GetMouseButtonDown(0);
            released |= Input.GetMouseButtonUp(0);
#endif

            if (pressed && released)
                return PointerEventData.FramePressState.PressedAndReleased;
            if (pressed)
                return PointerEventData.FramePressState.Pressed;
            if (released)
                return PointerEventData.FramePressState.Released;
            return PointerEventData.FramePressState.NotChanged;
        }
    }
}
