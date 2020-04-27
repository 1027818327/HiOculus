using ARdEZ.Render;
using UnityEngine;

namespace Oculus
{
    public class GamepadCtrl : MonoBehaviour
    {
        public GameObject leftGamepad;
        public GameObject rightGamepad;

        public GameObject rightHandTriggerObj;
        public GameObject rightHandTeleportObj;

        public GameObject leftHandTriggerObj;
        public GameObject leftHandTeleportObj;

        private Outline rightHandTriggerOutline;
        private Outline rightHandTeleportOutline;

        private Outline leftHandTriggerOutline;
        private Outline leftHandTeleportOutline;

        void Start() 
        {
            ShowGamepad();
            OpenTeleportBtnGuide();
        }


        public void ShowGamepad()
        {
            leftGamepad.SetActive(true);
            rightGamepad.SetActive(true);
        }

        public void HideGamepad() 
        {
            leftGamepad.SetActive(false);
            rightGamepad.SetActive(false);
        }

        public void CloseBtnGuide() 
        {
            if (rightHandTriggerOutline != null) 
            {
                rightHandTriggerOutline.enabled = false;
            }
            if (rightHandTeleportOutline != null)
            {
                rightHandTeleportOutline.enabled = false;
            }
            if (leftHandTriggerOutline != null)
            {
                leftHandTriggerOutline.enabled = false;
            }
            if (leftHandTeleportOutline != null)
            {
                leftHandTeleportOutline.enabled = false;
            }
        }

        private void OpenTeleportBtnGuide() 
        {
            if (rightHandTeleportOutline == null) 
            {
                rightHandTeleportOutline = rightHandTeleportObj.AddComponent<Outline>();
            }
            if (leftHandTeleportOutline == null)
            {
                leftHandTeleportOutline = leftHandTeleportObj.AddComponent<Outline>();
            }


            if (rightHandTeleportOutline != null)
            {
                rightHandTeleportOutline.OutlineColor = Color.yellow;
                rightHandTeleportOutline.enabled = true;
            }
            if (leftHandTeleportOutline != null)
            {
                leftHandTeleportOutline.OutlineColor = Color.yellow;
                leftHandTeleportOutline.enabled = true;
            }
        }

        private void OpenGrapBtnGuide()
        {
            if (rightHandTriggerOutline == null)
            {
                rightHandTriggerOutline = rightHandTriggerObj.AddComponent<Outline>();
            }
            if (leftHandTriggerOutline == null)
            {
                leftHandTriggerOutline = leftHandTriggerObj.AddComponent<Outline>();
            }

            if (rightHandTriggerOutline != null)
            {
                rightHandTriggerOutline.OutlineColor = Color.yellow;
                rightHandTriggerOutline.enabled = true;
            }
            if (leftHandTriggerOutline != null)
            {
                leftHandTriggerOutline.OutlineColor = Color.yellow;
                leftHandTriggerOutline.enabled = true;
            }
        }
    }
}
