using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Oculus
{
    public class OVR_HandEventListen : MonoBehaviour
    {
        public OVRInput.Controller controller;

        private List<Collider> enterObjs = new List<Collider>();

        private enum HandState 
        {
            Press,
            Release,
        }

        private bool isTouch;
        private HandState mState = HandState.Release;

        private void OnTriggerEnter(Collider other)
        {
            if (enterObjs.Contains(other)) 
            {
                return;
            }
            enterObjs.Add(other);

            isTouch = true;
            LogManager.instance.DebugLog("OnTriggerEnter " + other.name);
        }

        private void OnTriggerExit(Collider other)
        {
            enterObjs.Remove(other);
            isTouch = false;
            mState = HandState.Release;

            LogManager.instance.DebugLog("OnTriggerExit " + other.name);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!enterObjs.Contains(other)) 
            {
                return;
            }

            PointerEventData.FramePressState frameState = GetInputState();

            if (frameState == PointerEventData.FramePressState.Pressed)
            {
                mState = HandState.Press;
                LogManager.instance.DebugLog("press " + other.name);
            }
            if (frameState == PointerEventData.FramePressState.Released)
            {
                mState = HandState.Release;
                LogManager.instance.DebugLog("release " + other.name);
            }
        }

        protected PointerEventData.FramePressState GetInputState()
        {
            bool pressed = false;
            bool released = false;

            if (controller == OVRInput.Controller.LTouch)
            {
                pressed = (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) && OVRInput.GetDown(OVRInput.RawButton.LThumbstick));
                released = (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger) && OVRInput.GetUp(OVRInput.RawButton.LThumbstick));
            }
            else
            {
                pressed = (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger) && OVRInput.GetDown(OVRInput.RawButton.RThumbstick));
                released = (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger) && OVRInput.GetUp(OVRInput.RawButton.RThumbstick));
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
